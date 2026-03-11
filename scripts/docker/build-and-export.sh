#!/usr/bin/env bash
# =============================================================================
# Wekeza Bank — Build, Tag, and Export All Docker Images
#
# USAGE:
#   chmod +x scripts/docker/build-and-export.sh
#   ./scripts/docker/build-and-export.sh [--push] [--export] [--registry <registry>]
#
# OPTIONS:
#   --push      Push images to the registry (requires docker login first)
#   --export    Export images as .tar.gz files to ./docker-exports/
#   --registry  Registry prefix  (default: ghcr.io/eodenyire/wekeza)
#   --tag       Image tag        (default: latest)
#
# EXAMPLES:
#   # Build only (local tags)
#   ./scripts/docker/build-and-export.sh
#
#   # Build and push to GHCR
#   echo $GITHUB_TOKEN | docker login ghcr.io -u eodenyire --password-stdin
#   ./scripts/docker/build-and-export.sh --push
#
#   # Build and export as tar.gz (for offline distribution)
#   ./scripts/docker/build-and-export.sh --export
#
#   # Build, tag and push with a specific version tag
#   ./scripts/docker/build-and-export.sh --push --tag v1.0.0
# =============================================================================

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
REGISTRY="${REGISTRY:-ghcr.io/eodenyire/wekeza}"
TAG="${TAG:-latest}"
DO_PUSH=false
DO_EXPORT=false
EXPORT_DIR="${REPO_ROOT}/docker-exports"

while [[ $# -gt 0 ]]; do
    case $1 in
        --push)     DO_PUSH=true;   shift ;;
        --export)   DO_EXPORT=true; shift ;;
        --registry) REGISTRY="$2";  shift 2 ;;
        --tag)      TAG="$2";       shift 2 ;;
        *) echo "Unknown option: $1"; exit 1 ;;
    esac
done

echo "╔══════════════════════════════════════════════════════════════════╗"
echo "║  Wekeza Bank — Docker Image Build Pipeline                      ║"
echo "╚══════════════════════════════════════════════════════════════════╝"
echo "  Registry : $REGISTRY"
echo "  Tag      : $TAG"
echo "  Push     : $DO_PUSH"
echo "  Export   : $DO_EXPORT"
echo ""

cd "$REPO_ROOT"

# ---------------------------------------------------------------------------
# Build all services via docker compose
# ---------------------------------------------------------------------------
echo "▶  Building images via docker compose..."
docker compose -f docker-compose.full.yml build \
    --build-arg VITE_AUTH_MODE=real \
    --build-arg VITE_API_URL=/api
echo ""
echo "✔  All images built"
echo ""

# ---------------------------------------------------------------------------
# Tag with registry prefix
# ---------------------------------------------------------------------------
LOCAL_IMAGES=("wekeza-api:local" "wekeza-frontend:local")
REMOTE_NAMES=("wekeza-api" "wekeza-frontend")

for i in "${!LOCAL_IMAGES[@]}"; do
    local_img="${LOCAL_IMAGES[$i]}"
    remote="${REGISTRY}/${REMOTE_NAMES[$i]}:${TAG}"
    echo "  Tagging ${local_img} → ${remote}"
    docker tag "${local_img}" "${remote}" || echo "  ⚠  ${local_img} not found (build may have failed)"
done

# ---------------------------------------------------------------------------
# Push
# ---------------------------------------------------------------------------
if [[ "$DO_PUSH" == true ]]; then
    echo ""
    echo "▶  Pushing images to ${REGISTRY}..."
    for name in "${REMOTE_NAMES[@]}"; do
        remote="${REGISTRY}/${name}:${TAG}"
        echo "  → Pushing ${remote}"
        docker push "${remote}"
    done
    echo ""
    echo "✔  All images pushed"
    echo ""
    echo "  To pull on another machine:"
    for name in "${REMOTE_NAMES[@]}"; do
        echo "    docker pull ${REGISTRY}/${name}:${TAG}"
    done
fi

# ---------------------------------------------------------------------------
# Export as .tar.gz for offline distribution
# ---------------------------------------------------------------------------
if [[ "$DO_EXPORT" == true ]]; then
    echo ""
    echo "▶  Exporting images to ${EXPORT_DIR}/..."
    mkdir -p "${EXPORT_DIR}"

    # App images
    for name in "${REMOTE_NAMES[@]}"; do
        local_img="${name}:local"   # e.g. wekeza-api:local
        out="${EXPORT_DIR}/${name}-${TAG}.tar.gz"
        echo "  Saving ${local_img} → $(basename "${out}")"
        docker save "${local_img}" | gzip -9 > "${out}"
        echo "    Size: $(du -sh "${out}" | cut -f1)"
    done

    # Infrastructure base images (for fully-offline use)
    for base_img in "postgres:15-alpine" "redis:7-alpine" "nginx:1.25-alpine"; do
        safe_name="${base_img//:/-}"
        out="${EXPORT_DIR}/${safe_name}.tar.gz"
        if docker image inspect "${base_img}" &>/dev/null; then
            echo "  Saving ${base_img} → $(basename "${out}")"
            docker save "${base_img}" | gzip -9 > "${out}"
        else
            echo "  ⚠  ${base_img} not available locally (run: docker pull ${base_img})"
        fi
    done

    echo ""
    echo "✔  Export complete — files in ${EXPORT_DIR}/"
    echo ""
    ls -lh "${EXPORT_DIR}/"
    echo ""
    echo "  To load all images on another machine:"
    echo "    for f in docker-exports/*.tar.gz; do docker load -i \"\$f\"; done"
    echo "  Then start the stack:"
    echo "    docker compose -f docker-compose.full.yml up -d"
fi

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------
echo ""
echo "╔══════════════════════════════════════════════════════════════════╗"
echo "║  Local images                                                   ║"
echo "╚══════════════════════════════════════════════════════════════════╝"
docker images | grep -E "wekeza|postgres.*15|redis.*7|nginx.*1\.25" | \
    awk '{printf "  %-38s %-10s %s\n", $1":"$2, $4, $5}'
echo ""
echo "  ► Start full stack:   docker compose -f docker-compose.full.yml up -d"
echo "  ► Open portals:       http://localhost"
echo "  ► All passwords:      Admin@123"
echo ""
