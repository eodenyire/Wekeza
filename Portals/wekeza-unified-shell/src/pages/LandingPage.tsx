import React, { useState, useEffect, useRef } from 'react';
import {
  Modal,
  Form,
  Input,
  Button,
  Typography,
  message,
} from 'antd';
import {
  BankOutlined,
  MenuOutlined,
  CloseOutlined,
  PhoneOutlined,
  MailOutlined,
  EnvironmentOutlined,
  FacebookOutlined,
  TwitterOutlined,
  LinkedinOutlined,
  InstagramOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@store/authStore';
import { authService } from '@services/api';
import type { LoginCredentials } from '@app-types/index';

const { Title, Text } = Typography;

/* ── Animated counter hook ──────────────────────────────────────── */
function useCountUp(target: number, duration = 2000, start = false) {
  const [count, setCount] = useState(0);
  useEffect(() => {
    if (!start) return;
    let startTime: number | null = null;
    const step = (timestamp: number) => {
      if (!startTime) startTime = timestamp;
      const progress = Math.min((timestamp - startTime) / duration, 1);
      setCount(Math.floor(progress * target));
      if (progress < 1) requestAnimationFrame(step);
    };
    requestAnimationFrame(step);
  }, [target, duration, start]);
  return count;
}

function formatStat(value: number, target: number): string {
  if (value === 0) return '0';
  if (target >= 1_000_000_000) return `${(value / 1_000_000_000).toFixed(1)}B`;
  if (target >= 1_000_000) return `${(value / 1_000_000).toFixed(1)}M`;
  if (target >= 1_000) return `${(value / 1_000).toFixed(1)}K`;
  return String(value);
}

/* ── Inline style helpers ───────────────────────────────────────── */
const S = {
  primary: '#1a6b3a',
  primaryDark: '#14522d',
  accent: '#f0b429',
  white: '#fff',
  text: '#1a1a2e',
  textLight: '#555',
  bg: '#f8faf9',
  cardBg: '#fff',
  border: '#e2e8f0',
} as const;

export const LandingPage: React.FC = () => {
  const navigate = useNavigate();
  const { setAuth, setLoading, isLoading } = useAuthStore();

  const [menuOpen, setMenuOpen] = useState(false);
  const [loginVisible, setLoginVisible] = useState(false);
  const [registerVisible, setRegisterVisible] = useState(false);
  const [loginError, setLoginError] = useState<string | null>(null);
  const [statsStarted, setStatsStarted] = useState(false);
  const statsRef = useRef<HTMLDivElement>(null);

  const customers = useCountUp(50000, 2000, statsStarted);
  const secured = useCountUp(1_000_000_000, 2000, statsStarted);
  const support = useCountUp(24, 1500, statsStarted);

  const [loginForm] = Form.useForm<LoginCredentials>();
  const [registerForm] = Form.useForm();

  /* Trigger counter animation when hero stats scroll into view */
  useEffect(() => {
    const el = statsRef.current;
    if (!el) return;
    const observer = new IntersectionObserver(
      ([entry]) => { if (entry.isIntersecting) { setStatsStarted(true); observer.disconnect(); } },
      { threshold: 0.3 }
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, []);

  const scrollTo = (id: string) => {
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
    setMenuOpen(false);
  };

  /* ── Login submit ──────────────────────────────────────────────── */
  const handleLogin = async (values: LoginCredentials) => {
    try {
      setLoginError(null);
      setLoading(true);
      const response = await authService.login(values);
      setAuth(response);
      setLoginVisible(false);
      loginForm.resetFields();
      navigate('/dashboard', { replace: true });
    } catch (err: unknown) {
      const axiosErr = err as { response?: { status?: number; data?: { message?: string } } };
      if (axiosErr.response?.status === 401) {
        setLoginError('Invalid username or password. Please check your credentials.');
      } else if (axiosErr.response?.data?.message) {
        setLoginError(axiosErr.response.data.message);
      } else if (axiosErr.response) {
        setLoginError('Login failed. Please try again later.');
      } else {
        setLoginError('Unable to connect. Please check your connection and try again.');
      }
      setLoading(false);
    }
  };

  /* ── Register submit (UI only — backend integration pending) ───── */
  const handleRegister = (_values: Record<string, string>) => {
    message.info('Thank you for your interest! Our team will contact you shortly to complete your account setup.');
    setRegisterVisible(false);
    registerForm.resetFields();
  };

  /* ── Shared modal footer helpers ───────────────────────────────── */
  const switchToRegister = () => { setLoginVisible(false); setRegisterVisible(true); };
  const switchToLogin    = () => { setRegisterVisible(false); setLoginVisible(true); };

  /* ════════════════════════════════════════════════════════════════ */
  return (
    <div style={{ fontFamily: "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif", color: S.text, overflowX: 'hidden' }}>

      {/* ── NAVBAR ─────────────────────────────────────────────────── */}
      <nav style={{
        position: 'sticky', top: 0, zIndex: 1000,
        background: S.white,
        boxShadow: '0 2px 12px rgba(0,0,0,0.08)',
        padding: '0 24px',
      }}>
        <div style={{ maxWidth: 1200, margin: '0 auto', display: 'flex', alignItems: 'center', justifyContent: 'space-between', height: 64 }}>
          {/* Logo */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, cursor: 'pointer' }} onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}>
            <BankOutlined style={{ fontSize: 28, color: S.primary }} />
            <span style={{ fontWeight: 700, fontSize: 20, color: S.primary }}>Wekeza Bank</span>
          </div>

          {/* Desktop nav links */}
          <ul style={{ display: 'flex', listStyle: 'none', gap: 32, margin: 0, padding: 0 }} className="desktop-nav">
            {['home', 'services', 'features', 'about', 'contact'].map(id => (
              <li key={id}>
                <button
                  onClick={() => scrollTo(id)}
                  style={{ background: 'none', border: 'none', cursor: 'pointer', fontSize: 15, color: S.textLight, textTransform: 'capitalize', padding: '4px 0' }}
                >
                  {id === 'home' ? 'Home' : id.charAt(0).toUpperCase() + id.slice(1)}
                </button>
              </li>
            ))}
          </ul>

          {/* Nav actions */}
          <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
            <Button onClick={() => setLoginVisible(true)} style={{ borderColor: S.primary, color: S.primary }}>Login</Button>
            <Button type="primary" onClick={() => setRegisterVisible(true)} style={{ background: S.primary, borderColor: S.primary }}>Open Account</Button>
            <Button
              type="text"
              icon={menuOpen ? <CloseOutlined /> : <MenuOutlined />}
              onClick={() => setMenuOpen(o => !o)}
              style={{ display: 'none' }}
              className="hamburger-btn"
            />
          </div>
        </div>

        {/* Mobile menu */}
        {menuOpen && (
          <div style={{ background: S.white, borderTop: `1px solid ${S.border}`, padding: '16px 24px' }}>
            {['home', 'services', 'features', 'about', 'contact'].map(id => (
              <div key={id} style={{ padding: '10px 0', borderBottom: `1px solid ${S.border}` }}>
                <button onClick={() => scrollTo(id)} style={{ background: 'none', border: 'none', cursor: 'pointer', fontSize: 15, color: S.text, textTransform: 'capitalize', width: '100%', textAlign: 'left' }}>
                  {id.charAt(0).toUpperCase() + id.slice(1)}
                </button>
              </div>
            ))}
            <div style={{ marginTop: 16, display: 'flex', gap: 12 }}>
              <Button onClick={() => { setMenuOpen(false); setLoginVisible(true); }} style={{ flex: 1, borderColor: S.primary, color: S.primary }}>Login</Button>
              <Button type="primary" onClick={() => { setMenuOpen(false); setRegisterVisible(true); }} style={{ flex: 1, background: S.primary, borderColor: S.primary }}>Open Account</Button>
            </div>
          </div>
        )}
      </nav>

      {/* ── HERO ───────────────────────────────────────────────────── */}
      <section id="home" style={{ background: `linear-gradient(135deg, ${S.primary} 0%, #0d3d21 100%)`, padding: '80px 24px', color: S.white }}>
        <div style={{ maxWidth: 1200, margin: '0 auto', display: 'flex', gap: 48, alignItems: 'center', flexWrap: 'wrap' }}>
          {/* Hero text */}
          <div style={{ flex: '1 1 400px' }}>
            <Title level={1} style={{ color: S.white, fontSize: 'clamp(2rem, 5vw, 3.5rem)', lineHeight: 1.2, margin: 0 }}>
              Banking Made <span style={{ color: S.accent }}>Simple</span>
            </Title>
            <Text style={{ color: 'rgba(255,255,255,0.85)', fontSize: 18, display: 'block', marginTop: 16, lineHeight: 1.6 }}>
              Experience modern banking with seamless digital solutions. From savings to loans, we've got you covered.
            </Text>
            <div style={{ display: 'flex', gap: 16, marginTop: 32, flexWrap: 'wrap' }}>
              <Button size="large" onClick={() => setRegisterVisible(true)} style={{ background: S.accent, borderColor: S.accent, color: S.text, fontWeight: 600, height: 48, padding: '0 32px' }}>
                Get Started
              </Button>
              <Button size="large" ghost onClick={() => scrollTo('services')} style={{ height: 48, padding: '0 32px', borderColor: 'rgba(255,255,255,0.6)', color: S.white }}>
                Explore Services
              </Button>
            </div>

            {/* Stats */}
            <div ref={statsRef} style={{ display: 'flex', gap: 40, marginTop: 48, flexWrap: 'wrap' }}>
              {[
                { value: customers, target: 50000, label: 'Happy Customers' },
                { value: secured,   target: 1_000_000_000, label: 'Money Secured (KES)' },
                { value: support,   target: 24,  label: '24/7 Support' },
              ].map(({ value, target, label }) => (
                <div key={label} style={{ textAlign: 'center' }}>
                  <div style={{ fontSize: 36, fontWeight: 800, color: S.accent }}>{formatStat(value, target)}</div>
                  <div style={{ fontSize: 13, color: 'rgba(255,255,255,0.75)', marginTop: 4 }}>{label}</div>
                </div>
              ))}
            </div>
          </div>

          {/* Floating feature cards */}
          <div style={{ flex: '1 1 300px', display: 'flex', flexDirection: 'column', gap: 16 }}>
            {[
              { icon: '🛡️', title: 'Secure',  desc: 'Bank-grade security' },
              { icon: '⚡', title: 'Fast',    desc: 'Instant transactions' },
              { icon: '📱', title: 'Mobile',  desc: 'Bank anywhere' },
            ].map(({ icon, title, desc }) => (
              <div key={title} style={{
                background: 'rgba(255,255,255,0.12)',
                backdropFilter: 'blur(8px)',
                border: '1px solid rgba(255,255,255,0.2)',
                borderRadius: 16,
                padding: '20px 24px',
                display: 'flex',
                alignItems: 'center',
                gap: 16,
              }}>
                <span style={{ fontSize: 32 }}>{icon}</span>
                <div>
                  <div style={{ fontWeight: 700, fontSize: 16, color: S.white }}>{title}</div>
                  <div style={{ fontSize: 13, color: 'rgba(255,255,255,0.75)' }}>{desc}</div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ── SERVICES ───────────────────────────────────────────────── */}
      <section id="services" style={{ padding: '80px 24px', background: S.bg }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ textAlign: 'center', marginBottom: 56 }}>
            <Title level={2} style={{ margin: 0 }}>Our Services</Title>
            <Text style={{ color: S.textLight, fontSize: 16, marginTop: 8, display: 'block' }}>Comprehensive banking solutions tailored for you</Text>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: 24 }}>
            {[
              { icon: '🐷', title: 'Savings Accounts', desc: 'Secure your future with competitive interest rates and flexible savings options.', items: ['Up to 5% interest rate', 'No monthly fees', 'Free withdrawals'], cta: 'Learn More' },
              { icon: '💳', title: 'Current Accounts', desc: 'Perfect for daily transactions with unlimited access to your funds.', items: ['Free debit card', 'Mobile banking', 'Unlimited transactions'], cta: 'Learn More' },
              { icon: '🤝', title: 'Loans',            desc: 'Flexible loan options for personal, business, and mortgage needs.', items: ['Low interest rates', 'Quick approval', 'Flexible repayment'], cta: 'Apply Now' },
              { icon: '🏢', title: 'Business Banking', desc: 'Grow your business with tailored banking solutions and support.', items: ['Business accounts', 'Business loans', 'Merchant services'], cta: 'Get Started' },
              { icon: '🔄', title: 'Money Transfers',  desc: 'Send and receive money instantly with M-Pesa integration.', items: ['Instant transfers', 'M-Pesa integration', 'International transfers'], cta: 'Transfer Now' },
              { icon: '📱', title: 'Mobile Banking',   desc: 'Bank on the go with our powerful mobile application.', items: ['24/7 access', 'Secure transactions', 'Real-time updates'], cta: 'Download App' },
            ].map(({ icon, title, desc, items, cta }) => (
              <div key={title} style={{ background: S.cardBg, borderRadius: 16, padding: 28, boxShadow: '0 2px 16px rgba(0,0,0,0.06)', border: `1px solid ${S.border}`, display: 'flex', flexDirection: 'column' }}>
                <div style={{ fontSize: 40, marginBottom: 16 }}>{icon}</div>
                <Title level={4} style={{ margin: '0 0 8px 0' }}>{title}</Title>
                <Text style={{ color: S.textLight, lineHeight: 1.6, display: 'block', marginBottom: 16 }}>{desc}</Text>
                <ul style={{ listStyle: 'none', padding: 0, margin: '0 0 20px 0', flex: 1 }}>
                  {items.map(item => (
                    <li key={item} style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 8, color: S.textLight, fontSize: 14 }}>
                      <span style={{ color: S.primary, fontWeight: 700 }}>✓</span> {item}
                    </li>
                  ))}
                </ul>
                <Button type="link" onClick={() => setRegisterVisible(true)} style={{ padding: 0, color: S.primary, fontWeight: 600, textAlign: 'left' }}>
                  {cta} →
                </Button>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ── FEATURES ───────────────────────────────────────────────── */}
      <section id="features" style={{ padding: '80px 24px', background: S.white }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ textAlign: 'center', marginBottom: 56 }}>
            <Title level={2} style={{ margin: 0 }}>Why Choose Wekeza Bank</Title>
            <Text style={{ color: S.textLight, fontSize: 16, marginTop: 8, display: 'block' }}>Modern banking backed by cutting-edge technology</Text>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 32 }}>
            {[
              { icon: '🔒', title: 'Bank-Grade Security',  desc: 'Your money is protected with military-grade encryption and multi-factor authentication.' },
              { icon: '📊', title: 'Real-Time Analytics',  desc: 'Track your spending, savings, and investments with powerful analytics tools.' },
              { icon: '🎧', title: '24/7 Support',         desc: 'Our dedicated support team is always available to help you with any questions.' },
              { icon: '🌍', title: 'Multi-Currency',       desc: 'Hold and transact in multiple currencies including KES, USD, EUR, and GBP.' },
            ].map(({ icon, title, desc }) => (
              <div key={title} style={{ textAlign: 'center', padding: 24 }}>
                <div style={{ fontSize: 48, marginBottom: 16 }}>{icon}</div>
                <Title level={4} style={{ margin: '0 0 12px 0' }}>{title}</Title>
                <Text style={{ color: S.textLight, lineHeight: 1.6 }}>{desc}</Text>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ── DASHBOARD PREVIEW ──────────────────────────────────────── */}
      <section id="about" style={{ padding: '80px 24px', background: S.bg }}>
        <div style={{ maxWidth: 1200, margin: '0 auto', display: 'flex', gap: 64, alignItems: 'center', flexWrap: 'wrap' }}>
          <div style={{ flex: '1 1 360px' }}>
            <Title level={2} style={{ margin: '0 0 16px 0' }}>Your Money, Your Control</Title>
            <Text style={{ color: S.textLight, lineHeight: 1.7, fontSize: 16, display: 'block', marginBottom: 24 }}>
              Access your complete banking dashboard with real-time updates, transaction history, and personalized insights.
            </Text>
            <ul style={{ listStyle: 'none', padding: 0, margin: '0 0 32px 0' }}>
              {['Real-time balance updates', 'Instant transaction notifications', 'Customizable spending alerts', 'Detailed financial reports'].map(item => (
                <li key={item} style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 12, fontSize: 15 }}>
                  <span style={{ background: S.primary, color: S.white, borderRadius: '50%', width: 22, height: 22, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, flexShrink: 0 }}>✓</span>
                  {item}
                </li>
              ))}
            </ul>
            <Button size="large" type="primary" onClick={() => setLoginVisible(true)} style={{ background: S.primary, borderColor: S.primary, height: 48, padding: '0 32px' }}>
              Access Dashboard
            </Button>
          </div>

          {/* Mockup window */}
          <div style={{ flex: '1 1 320px' }}>
            <div style={{ background: S.white, borderRadius: 16, boxShadow: '0 12px 40px rgba(0,0,0,0.12)', overflow: 'hidden', border: `1px solid ${S.border}` }}>
              <div style={{ background: '#f1f5f4', padding: '12px 16px', display: 'flex', alignItems: 'center', gap: 8 }}>
                <span style={{ width: 12, height: 12, borderRadius: '50%', background: '#ff5f57', display: 'inline-block' }} />
                <span style={{ width: 12, height: 12, borderRadius: '50%', background: '#ffbd2e', display: 'inline-block' }} />
                <span style={{ width: 12, height: 12, borderRadius: '50%', background: '#28c840', display: 'inline-block' }} />
                <span style={{ flex: 1, textAlign: 'center', fontSize: 13, color: S.textLight, fontWeight: 500 }}>Banking Dashboard</span>
              </div>
              <div style={{ padding: 24 }}>
                <div style={{ background: `linear-gradient(135deg, ${S.primary}, #0d3d21)`, borderRadius: 12, padding: 20, color: S.white, marginBottom: 20 }}>
                  <div style={{ fontSize: 13, opacity: 0.8, marginBottom: 4 }}>Total Balance</div>
                  <div style={{ fontSize: 28, fontWeight: 800 }}>KES 1,234,567.89</div>
                </div>
                <div style={{ display: 'flex', gap: 12 }}>
                  {['📤 Send', '📥 Receive', '🔄 Transfer'].map(action => (
                    <div key={action} style={{ flex: 1, background: S.bg, borderRadius: 10, padding: '12px 8px', textAlign: 'center', fontSize: 13, fontWeight: 600, cursor: 'pointer', border: `1px solid ${S.border}` }}>
                      {action}
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* ── CTA ────────────────────────────────────────────────────── */}
      <section style={{ background: `linear-gradient(135deg, ${S.primary} 0%, #0d3d21 100%)`, padding: '80px 24px', textAlign: 'center' }}>
        <Title level={2} style={{ color: S.white, margin: '0 0 16px 0' }}>Ready to Get Started?</Title>
        <Text style={{ color: 'rgba(255,255,255,0.85)', fontSize: 18, display: 'block', marginBottom: 32 }}>
          Open your account today and experience banking like never before
        </Text>
        <Button size="large" onClick={() => setRegisterVisible(true)} style={{ background: S.white, borderColor: S.white, color: S.primary, fontWeight: 700, height: 52, padding: '0 40px', fontSize: 16 }}>
          🚀 Open Account Now
        </Button>
      </section>

      {/* ── CONTACT ────────────────────────────────────────────────── */}
      <section id="contact" style={{ padding: '80px 24px', background: S.bg }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ textAlign: 'center', marginBottom: 56 }}>
            <Title level={2} style={{ margin: 0 }}>Get In Touch</Title>
            <Text style={{ color: S.textLight, fontSize: 16, marginTop: 8, display: 'block' }}>We're here to help with all your banking needs</Text>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: 48 }}>
            {/* Contact info */}
            <div style={{ display: 'flex', flexDirection: 'column', gap: 32 }}>
              {[
                { icon: <PhoneOutlined />, title: 'Phone',    value: '0716478835' },
                { icon: <MailOutlined />,  title: 'Email',    value: 'support@wekeza.com' },
                { icon: <EnvironmentOutlined />, title: 'Location', value: 'Nairobi, Kenya' },
              ].map(({ icon, title, value }) => (
                <div key={title} style={{ display: 'flex', alignItems: 'flex-start', gap: 16 }}>
                  <div style={{ width: 48, height: 48, background: S.primary, borderRadius: 12, display: 'flex', alignItems: 'center', justifyContent: 'center', color: S.white, fontSize: 20, flexShrink: 0 }}>
                    {icon}
                  </div>
                  <div>
                    <div style={{ fontWeight: 700, fontSize: 15, marginBottom: 4 }}>{title}</div>
                    <div style={{ color: S.textLight }}>{value}</div>
                  </div>
                </div>
              ))}
            </div>

            {/* Contact form */}
            <Form layout="vertical" onFinish={() => message.info('Thank you! Our team will reach out to you shortly.')}>
              <Form.Item name="name" rules={[{ required: true, message: 'Please enter your name' }]}>
                <Input placeholder="Your Name" size="large" style={{ borderRadius: 8 }} />
              </Form.Item>
              <Form.Item name="email" rules={[{ required: true, type: 'email', message: 'Please enter a valid email' }]}>
                <Input placeholder="Your Email" size="large" style={{ borderRadius: 8 }} />
              </Form.Item>
              <Form.Item name="message" rules={[{ required: true, message: 'Please enter your message' }]}>
                <Input.TextArea placeholder="Your Message" rows={5} style={{ borderRadius: 8 }} />
              </Form.Item>
              <Button htmlType="submit" type="primary" size="large" block style={{ background: S.primary, borderColor: S.primary, borderRadius: 8, height: 48 }}>
                Send Message
              </Button>
            </Form>
          </div>
        </div>
      </section>

      {/* ── FOOTER ─────────────────────────────────────────────────── */}
      <footer style={{ background: '#0d1f14', color: 'rgba(255,255,255,0.75)', padding: '60px 24px 24px' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 40, marginBottom: 40 }}>
            <div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12 }}>
                <BankOutlined style={{ fontSize: 24, color: S.accent }} />
                <span style={{ fontWeight: 700, fontSize: 18, color: S.white }}>Wekeza Bank</span>
              </div>
              <Text style={{ color: 'rgba(255,255,255,0.6)', lineHeight: 1.6, display: 'block', marginBottom: 16 }}>
                Modern banking solutions for Kenya and beyond.
              </Text>
              <div style={{ display: 'flex', gap: 12 }}>
                {[
                  { Icon: FacebookOutlined,  label: 'Facebook' },
                  { Icon: TwitterOutlined,   label: 'Twitter' },
                  { Icon: LinkedinOutlined,  label: 'LinkedIn' },
                  { Icon: InstagramOutlined, label: 'Instagram' },
                ].map(({ Icon, label }) => (
                  <a key={label} href="#" aria-label={label} onClick={(e) => e.preventDefault()} style={{ color: 'rgba(255,255,255,0.6)', fontSize: 18 }}><Icon /></a>
                ))}
              </div>
            </div>
            <div>
              <Title level={5} style={{ color: S.white, marginBottom: 16 }}>Quick Links</Title>
              <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
                {[['Services', 'services'], ['Features', 'features'], ['About Us', 'about'], ['Contact', 'contact']].map(([label, id]) => (
                  <li key={id} style={{ marginBottom: 10 }}>
                    <button onClick={() => scrollTo(id)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'rgba(255,255,255,0.6)', fontSize: 14, padding: 0 }}>
                      {label}
                    </button>
                  </li>
                ))}
              </ul>
            </div>
            <div>
              <Title level={5} style={{ color: S.white, marginBottom: 16 }}>Services</Title>
              <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
                {[
                  ['Savings Accounts', 'services'],
                  ['Current Accounts', 'services'],
                  ['Loans', 'services'],
                  ['Cards', 'services'],
                ].map(([item, section]) => (
                  <li key={item} style={{ marginBottom: 10 }}>
                    <button onClick={() => scrollTo(section)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'rgba(255,255,255,0.6)', fontSize: 14, padding: 0 }}>{item}</button>
                  </li>
                ))}
              </ul>
            </div>
            <div>
              <Title level={5} style={{ color: S.white, marginBottom: 16 }}>Support</Title>
              <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
                {['Help Center', 'Privacy Policy', 'Terms of Service', 'FAQs'].map(item => (
                  <li key={item} style={{ marginBottom: 10 }}>
                    <button onClick={() => scrollTo('contact')} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'rgba(255,255,255,0.6)', fontSize: 14, padding: 0 }}>{item}</button>
                  </li>
                ))}
              </ul>
            </div>
          </div>
          <div style={{ borderTop: '1px solid rgba(255,255,255,0.1)', paddingTop: 24, textAlign: 'center', color: 'rgba(255,255,255,0.45)', fontSize: 14 }}>
            © 2026 Wekeza Bank. All rights reserved. | Owner: Emmanuel Odenyire
          </div>
        </div>
      </footer>

      {/* ── LOGIN MODAL ────────────────────────────────────────────── */}
      <Modal
        title={<span style={{ fontSize: 20, fontWeight: 700 }}>Welcome Back</span>}
        open={loginVisible}
        onCancel={() => { setLoginVisible(false); setLoginError(null); loginForm.resetFields(); }}
        footer={null}
        width={400}
        centered
      >
        {loginError && (
          <div style={{ background: '#fff2f0', border: '1px solid #ffccc7', borderRadius: 8, padding: '10px 14px', marginBottom: 16, color: '#a8071a', fontSize: 14 }}>
            {loginError}
          </div>
        )}
        <Form form={loginForm} layout="vertical" onFinish={handleLogin}>
          <Form.Item label="Username" name="username" rules={[{ required: true, message: 'Please enter your username' }]}>
            <Input size="large" placeholder="Username" />
          </Form.Item>
          <Form.Item label="Password" name="password" rules={[{ required: true, message: 'Please enter your password' }]}>
            <Input.Password size="large" placeholder="Password" />
          </Form.Item>
          <Form.Item style={{ marginBottom: 8 }}>
            <Button htmlType="submit" type="primary" size="large" block loading={isLoading} style={{ background: S.primary, borderColor: S.primary }}>
              Login
            </Button>
          </Form.Item>
        </Form>
        <div style={{ textAlign: 'center', color: S.textLight, fontSize: 14 }}>
          Don't have an account?{' '}
          <button onClick={switchToRegister} style={{ background: 'none', border: 'none', cursor: 'pointer', color: S.primary, fontWeight: 600 }}>
            Sign up
          </button>
        </div>
      </Modal>

      {/* ── REGISTER MODAL ─────────────────────────────────────────── */}
      <Modal
        title={<span style={{ fontSize: 20, fontWeight: 700 }}>Open Your Account</span>}
        open={registerVisible}
        onCancel={() => { setRegisterVisible(false); registerForm.resetFields(); }}
        footer={null}
        width={520}
        centered
        styles={{ body: { maxHeight: '70vh', overflowY: 'auto' } }}
      >
        <Form form={registerForm} layout="vertical" onFinish={handleRegister}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 16px' }}>
            <Form.Item label="First Name" name="firstName" rules={[{ required: true }]}>
              <Input size="large" placeholder="First Name" />
            </Form.Item>
            <Form.Item label="Last Name" name="lastName" rules={[{ required: true }]}>
              <Input size="large" placeholder="Last Name" />
            </Form.Item>
            <Form.Item label="Username" name="username" rules={[{ required: true }]}>
              <Input size="large" placeholder="Username" />
            </Form.Item>
            <Form.Item label="Email" name="email" rules={[{ required: true, type: 'email' }]}>
              <Input size="large" placeholder="Email" />
            </Form.Item>
            <Form.Item label="Phone Number" name="phone" rules={[{ required: true }]}>
              <Input size="large" placeholder="Phone Number" />
            </Form.Item>
            <Form.Item label="ID Number" name="idNumber" rules={[{ required: true }]}>
              <Input size="large" placeholder="ID Number" />
            </Form.Item>
          </div>
          <Form.Item label="Date of Birth" name="dob" rules={[{ required: true }]}>
            <Input size="large" type="date" />
          </Form.Item>
          <Form.Item label="Address" name="address" rules={[{ required: true }]}>
            <Input.TextArea rows={3} placeholder="Address" />
          </Form.Item>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 16px' }}>
            <Form.Item label="Password" name="password" rules={[{ required: true, min: 8, message: 'Min 8 characters' }]}>
              <Input.Password size="large" placeholder="Password" />
            </Form.Item>
            <Form.Item
              label="Confirm Password"
              name="confirmPassword"
              dependencies={['password']}
              rules={[
                { required: true },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    return !value || getFieldValue('password') === value
                      ? Promise.resolve()
                      : Promise.reject(new Error('Passwords do not match'));
                  },
                }),
              ]}
            >
              <Input.Password size="large" placeholder="Confirm Password" />
            </Form.Item>
          </div>
          <Form.Item style={{ marginBottom: 8 }}>
            <Button htmlType="submit" type="primary" size="large" block style={{ background: S.primary, borderColor: S.primary }}>
              Create Account
            </Button>
          </Form.Item>
        </Form>
        <div style={{ textAlign: 'center', color: S.textLight, fontSize: 14 }}>
          Already have an account?{' '}
          <button onClick={switchToLogin} style={{ background: 'none', border: 'none', cursor: 'pointer', color: S.primary, fontWeight: 600 }}>
            Login
          </button>
        </div>
      </Modal>

      {/* ── Responsive CSS ─────────────────────────────────────────── */}
      <style>{`
        @media (max-width: 768px) {
          .desktop-nav { display: none !important; }
          .hamburger-btn { display: flex !important; }
        }
      `}</style>
    </div>
  );
};
