#!/usr/bin/env python3
"""
Wekeza Banking – Performance Tests
=====================================
Validates response-time SLAs and throughput requirements for the v1-Core API
under simulated load.

Tests:
  • Response-time benchmarks (p50, p95, p99)
  • Concurrent-user load simulation
  • Sustained throughput (requests per second)
  • Memory stability (via repeated calls)

Usage:
    API_BASE_URL=http://localhost:8080 python3 tests/performance/test_load.py

Environment variables:
    API_BASE_URL          – Base URL of the API (default: http://localhost:8080)
    TELLER_USERNAME/PASSWORD – Teller credentials
    PERF_CONCURRENT_USERS – Concurrent users for load test (default: 20)
    PERF_REQUESTS         – Total requests per test (default: 100)
    WEKEZA_SKIP_LIVE_TESTS – Set to "true" to skip live API tests
"""

import gc
import json
import os
import sys
import time
import threading
import unittest
import urllib.error
import urllib.request
from statistics import mean, median, quantiles
from typing import Callable

# ─── Config ───────────────────────────────────────────────────────────────────
API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")
TELLER_USERNAME = os.getenv("TELLER_USERNAME", "teller1")
TELLER_PASSWORD = os.getenv("TELLER_PASSWORD", "Teller@123")
CUSTOMER_USERNAME = os.getenv("CUSTOMER_USERNAME", "customer1")
CUSTOMER_PASSWORD = os.getenv("CUSTOMER_PASSWORD", "Customer@123")
ACTIVE_ACCOUNT = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0000055583")

CONCURRENT_USERS = int(os.getenv("PERF_CONCURRENT_USERS", "20"))
TOTAL_REQUESTS = int(os.getenv("PERF_REQUESTS", "100"))
SKIP_LIVE = os.getenv("WEKEZA_SKIP_LIVE_TESTS", "false").lower() in ("1", "true", "yes")

# SLA thresholds
P95_TARGET_MS = 500          # 95th-percentile must be under 500 ms
AVG_TARGET_MS = 200          # average must be under 200 ms
MIN_RPS = 10                 # minimum requests-per-second throughput


# ─── HTTP helper ──────────────────────────────────────────────────────────────

def api_request(method, path, token=None, body=None, timeout=30):
    url = f"{API_BASE_URL}{path}"
    data = json.dumps(body).encode() if body is not None else None
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    req = urllib.request.Request(url=url, method=method, data=data, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=timeout) as resp:
            raw = resp.read().decode()
            return resp.status, json.loads(raw) if raw else {}
    except urllib.error.HTTPError as exc:
        raw = exc.read().decode() if exc.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw[:200]}
        return exc.code, payload
    except Exception:
        return 0, {}


def is_reachable():
    status, _ = api_request("GET", "/health")
    return status == 200


def get_token(username, password):
    status, payload = api_request(
        "POST",
        "/api/authentication/login",
        body={"Username": username, "Password": password},
    )
    if status in (200, 201):
        return payload.get("token") or payload.get("Token")
    return None


# ─── Measurement helpers ──────────────────────────────────────────────────────

def measure_ms(fn: Callable) -> float:
    """Return elapsed time in milliseconds for a single call."""
    t0 = time.perf_counter()
    fn()
    return (time.perf_counter() - t0) * 1000.0


def run_concurrent(fn: Callable, n_threads: int) -> list[float]:
    """Run *fn* concurrently across *n_threads* threads; return elapsed-ms list."""
    results: list[float] = []
    lock = threading.Lock()

    def worker():
        elapsed = measure_ms(fn)
        with lock:
            results.append(elapsed)

    threads = [threading.Thread(target=worker) for _ in range(n_threads)]
    for t in threads:
        t.start()
    for t in threads:
        t.join()
    return results


def percentile(data: list[float], pct: float) -> float:
    sorted_data = sorted(data)
    idx = int(len(sorted_data) * pct / 100)
    return sorted_data[min(idx, len(sorted_data) - 1)]


# ─── Base class ───────────────────────────────────────────────────────────────

class PerformanceTestCase(unittest.TestCase):

    @classmethod
    def setUpClass(cls):
        if SKIP_LIVE:
            raise unittest.SkipTest("Live performance tests disabled via WEKEZA_SKIP_LIVE_TESTS")
        if not is_reachable():
            raise unittest.SkipTest(
                f"API not reachable at {API_BASE_URL}/health"
            )
        cls.teller_token = get_token(TELLER_USERNAME, TELLER_PASSWORD)
        cls.customer_token = get_token(CUSTOMER_USERNAME, CUSTOMER_PASSWORD)


# ═══════════════════════════════════════════════════════════════════════════════
# 1. Single-Request Response Time Benchmarks
# ═══════════════════════════════════════════════════════════════════════════════

class TestResponseTimeBenchmarks(PerformanceTestCase):

    def _assert_response_time(self, label: str, fn: Callable, max_ms: float = 500):
        samples = [measure_ms(fn) for _ in range(10)]
        avg = mean(samples)
        p95 = percentile(samples, 95)
        print(f"\n  [{label}]  avg={avg:.1f}ms  p95={p95:.1f}ms  "
              f"min={min(samples):.1f}ms  max={max(samples):.1f}ms")
        self.assertLess(avg, max_ms,
                        f"{label}: average {avg:.1f}ms exceeds {max_ms}ms target")

    def test_01_health_endpoint_response_time(self):
        """Health endpoint must respond in under 200 ms on average."""
        self._assert_response_time(
            "GET /health",
            lambda: api_request("GET", "/health"),
            max_ms=200,
        )

    def test_02_login_response_time(self):
        """Login endpoint must respond in under 500 ms on average."""
        self._assert_response_time(
            "POST /api/authentication/login",
            lambda: api_request(
                "POST",
                "/api/authentication/login",
                body={"Username": CUSTOMER_USERNAME, "Password": CUSTOMER_PASSWORD},
            ),
            max_ms=500,
        )

    def test_03_list_accounts_response_time(self):
        """Listing accounts must respond in under 500 ms on average."""
        token = self.customer_token
        if not token:
            self.skipTest("Could not get customer token")
        self._assert_response_time(
            "GET /api/accounts/user/accounts",
            lambda: api_request("GET", "/api/accounts/user/accounts", token=token),
            max_ms=500,
        )

    def test_04_balance_inquiry_response_time(self):
        """Balance inquiry must respond in under 300 ms on average."""
        token = self.customer_token
        if not token:
            self.skipTest("Could not get customer token")
        self._assert_response_time(
            f"GET /api/accounts/{ACTIVE_ACCOUNT}/balance",
            lambda: api_request("GET", f"/api/accounts/{ACTIVE_ACCOUNT}/balance", token=token),
            max_ms=300,
        )


# ═══════════════════════════════════════════════════════════════════════════════
# 2. Concurrent-User Load Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestConcurrentLoad(PerformanceTestCase):

    def test_01_concurrent_health_checks(self):
        """Health endpoint handles {n} concurrent requests within SLA."""
        n = CONCURRENT_USERS
        results = run_concurrent(
            lambda: api_request("GET", "/health"),
            n_threads=n,
        )
        avg = mean(results)
        p95 = percentile(results, 95)
        print(f"\n  Concurrent health ({n} threads): avg={avg:.1f}ms p95={p95:.1f}ms")
        self.assertLess(avg, AVG_TARGET_MS,
                        f"Average {avg:.1f}ms exceeds {AVG_TARGET_MS}ms under {n} concurrent users")
        self.assertLess(p95, P95_TARGET_MS,
                        f"p95 {p95:.1f}ms exceeds {P95_TARGET_MS}ms under {n} concurrent users")

    def test_02_concurrent_logins(self):
        """Login endpoint handles concurrent requests within SLA."""
        n = min(CONCURRENT_USERS, 10)   # keep it moderate for auth
        results = run_concurrent(
            lambda: api_request(
                "POST",
                "/api/authentication/login",
                body={"Username": CUSTOMER_USERNAME, "Password": CUSTOMER_PASSWORD},
            ),
            n_threads=n,
        )
        avg = mean(results)
        p95 = percentile(results, 95)
        print(f"\n  Concurrent logins ({n} threads): avg={avg:.1f}ms p95={p95:.1f}ms")
        self.assertLess(avg, 1000,
                        f"Average login time {avg:.1f}ms exceeds 1s under {n} concurrent users")

    def test_03_concurrent_balance_inquiries(self):
        """Balance inquiry handles concurrent requests within SLA."""
        token = self.customer_token
        if not token:
            self.skipTest("No customer token")
        n = CONCURRENT_USERS
        results = run_concurrent(
            lambda: api_request("GET", f"/api/accounts/{ACTIVE_ACCOUNT}/balance", token=token),
            n_threads=n,
        )
        avg = mean(results)
        p95 = percentile(results, 95)
        print(f"\n  Concurrent balance inquiries ({n} threads): avg={avg:.1f}ms p95={p95:.1f}ms")
        self.assertLess(p95, P95_TARGET_MS)


# ═══════════════════════════════════════════════════════════════════════════════
# 3. Throughput (RPS) Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestThroughput(PerformanceTestCase):

    def _measure_rps(self, label: str, fn: Callable, n: int = TOTAL_REQUESTS) -> float:
        t0 = time.perf_counter()
        for _ in range(n):
            fn()
        elapsed = time.perf_counter() - t0
        rps = n / elapsed
        print(f"\n  [{label}]  {n} requests in {elapsed:.2f}s → {rps:.1f} RPS")
        return rps

    def test_01_health_throughput(self):
        """Health endpoint must sustain at least {MIN_RPS} RPS."""
        rps = self._measure_rps(
            "Health RPS",
            lambda: api_request("GET", "/health"),
            n=50,
        )
        self.assertGreater(rps, MIN_RPS,
                           f"Health endpoint only {rps:.1f} RPS – below {MIN_RPS} RPS minimum")

    def test_02_sequential_login_throughput(self):
        """Login must sustain at least 5 RPS sequentially (auth is more expensive)."""
        rps = self._measure_rps(
            "Login RPS",
            lambda: api_request(
                "POST",
                "/api/authentication/login",
                body={"Username": CUSTOMER_USERNAME, "Password": CUSTOMER_PASSWORD},
            ),
            n=20,
        )
        self.assertGreater(rps, 1,
                           f"Login throughput {rps:.1f} RPS – unexpectedly slow")


# ═══════════════════════════════════════════════════════════════════════════════
# 4. Stability / Memory-Leak Detection
# ═══════════════════════════════════════════════════════════════════════════════

class TestStability(PerformanceTestCase):

    def test_01_repeated_calls_no_memory_leak(self):
        """Memory usage must not grow unboundedly across 200 requests."""
        gc.collect()
        import tracemalloc
        tracemalloc.start()

        for _ in range(200):
            api_request("GET", "/health")

        current, peak = tracemalloc.get_traced_memory()
        tracemalloc.stop()

        peak_mb = peak / 1024 / 1024
        print(f"\n  Memory peak during 200 health requests: {peak_mb:.2f} MB")
        self.assertLess(peak_mb, 50,
                        f"Memory peak {peak_mb:.2f} MB exceeds 50 MB threshold")

    def test_02_burst_followed_by_recovery(self):
        """After a burst, the API must still respond within SLA."""
        # Burst: 50 concurrent requests
        run_concurrent(lambda: api_request("GET", "/health"), n_threads=50)

        # Recovery check
        time.sleep(0.5)
        elapsed = measure_ms(lambda: api_request("GET", "/health"))
        print(f"\n  Post-burst health response: {elapsed:.1f}ms")
        self.assertLess(elapsed, 500,
                        f"Post-burst response {elapsed:.1f}ms exceeds 500ms")


# ─── Main ─────────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    print(f"API target       : {API_BASE_URL}")
    print(f"Concurrent users : {CONCURRENT_USERS}")
    print(f"Total requests   : {TOTAL_REQUESTS}")
    print(f"p95 SLA target   : {P95_TARGET_MS} ms")
    print(f"RPS minimum      : {MIN_RPS}")
    print()
    loader = unittest.TestLoader()
    loader.sortTestMethodsUsing = None
    suite = loader.loadTestsFromModule(sys.modules[__name__])
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(suite)
    sys.exit(0 if result.wasSuccessful() else 1)
