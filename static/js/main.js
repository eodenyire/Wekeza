// API Base URL
const API_BASE_URL = window.location.origin;
let authToken = localStorage.getItem('authToken');
let currentUser = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
    animateStats();
    setupEventListeners();
    checkAuthStatus();
});

// Initialize the application
function initializeApp() {
    // Smooth scrolling
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });
    
    // Mobile menu toggle
    const hamburger = document.getElementById('hamburger');
    const navMenu = document.getElementById('navMenu');
    
    if (hamburger) {
        hamburger.addEventListener('click', () => {
            navMenu.classList.toggle('active');
        });
    }
    
    // Close modals on outside click
    window.addEventListener('click', (e) => {
        if (e.target.classList.contains('modal')) {
            closeModal(e.target.id);
        }
    });
}

// Animate statistics counter
function animateStats() {
    const stats = document.querySelectorAll('.stat-number');
    
    stats.forEach(stat => {
        const target = parseInt(stat.getAttribute('data-target'));
        const duration = 2000;
        const increment = target / (duration / 16);
        let current = 0;
        
        const updateCounter = () => {
            current += increment;
            if (current < target) {
                stat.textContent = Math.floor(current).toLocaleString();
                requestAnimationFrame(updateCounter);
            } else {
                stat.textContent = target.toLocaleString();
            }
        };
        
        // Start animation when element is in viewport
        const observer = new IntersectionObserver((entries) => {
            if (entries[0].isIntersecting) {
                updateCounter();
                observer.disconnect();
            }
        });
        
        observer.observe(stat);
    });
}

// Setup event listeners
function setupEventListeners() {
    // Login form
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }
    
    // Register form
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }
    
    // Contact form
    const contactForm = document.getElementById('contactForm');
    if (contactForm) {
        contactForm.addEventListener('submit', handleContactForm);
    }
}

// Check authentication status
async function checkAuthStatus() {
    if (authToken) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/customers/`, {
                headers: {
                    'Authorization': `Bearer ${authToken}`
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                currentUser = data.results ? data.results[0] : data[0];
                updateUIForLoggedInUser();
            } else {
                localStorage.removeItem('authToken');
                authToken = null;
            }
        } catch (error) {
            console.error('Auth check failed:', error);
        }
    }
}

// Update UI for logged-in user
function updateUIForLoggedInUser() {
    const navActions = document.querySelector('.nav-actions');
    if (navActions && currentUser) {
        navActions.innerHTML = `
            <span>Welcome, ${currentUser.user.first_name}!</span>
            <button class="btn btn-primary" onclick="loadDashboard()">Dashboard</button>
            <button class="btn btn-outline" onclick="logout()">Logout</button>
        `;
    }
}

// Modal functions
function showModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('active');
        document.body.style.overflow = 'auto';
    }
}

function showLoginModal() {
    closeModal('registerModal');
    showModal('loginModal');
}

function showRegisterModal() {
    closeModal('loginModal');
    showModal('registerModal');
}

function switchToRegister() {
    closeModal('loginModal');
    showModal('registerModal');
}

function switchToLogin() {
    closeModal('registerModal');
    showModal('loginModal');
}

// Handle login
async function handleLogin(e) {
    e.preventDefault();
    
    const username = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;
    
    try {
        const response = await fetch(`${API_BASE_URL}/api/token/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });
        
        if (response.ok) {
            const data = await response.json();
            authToken = data.access;
            localStorage.setItem('authToken', authToken);
            
            showNotification('Login successful!', 'success');
            closeModal('loginModal');
            
            // Get user data and update UI
            await checkAuthStatus();
            
            // Load dashboard after short delay
            setTimeout(() => {
                loadDashboard();
            }, 1000);
        } else {
            const error = await response.json();
            showNotification(error.detail || 'Login failed', 'error');
        }
    } catch (error) {
        console.error('Login error:', error);
        showNotification('An error occurred. Please try again.', 'error');
    }
}

// Handle registration
async function handleRegister(e) {
    e.preventDefault();
    
    const formData = {
        first_name: document.getElementById('regFirstName').value,
        last_name: document.getElementById('regLastName').value,
        username: document.getElementById('regUsername').value,
        email: document.getElementById('regEmail').value,
        phone_number: document.getElementById('regPhone').value,
        id_number: document.getElementById('regIdNumber').value,
        date_of_birth: document.getElementById('regDob').value,
        address: document.getElementById('regAddress').value,
        password: document.getElementById('regPassword').value,
        password2: document.getElementById('regPassword2').value
    };
    
    if (formData.password !== formData.password2) {
        showNotification('Passwords do not match', 'error');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/api/register/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });
        
        if (response.ok) {
            const data = await response.json();
            showNotification('Registration successful! Please login.', 'success');
            closeModal('registerModal');
            
            // Auto-fill login form
            document.getElementById('loginUsername').value = formData.username;
            
            // Show login modal after short delay
            setTimeout(() => {
                showLoginModal();
            }, 1000);
        } else {
            const error = await response.json();
            const errorMessage = Object.values(error).flat().join(', ');
            showNotification(errorMessage || 'Registration failed', 'error');
        }
    } catch (error) {
        console.error('Registration error:', error);
        showNotification('An error occurred. Please try again.', 'error');
    }
}

// Handle contact form
function handleContactForm(e) {
    e.preventDefault();
    showNotification('Thank you for your message! We will get back to you soon.', 'success');
    e.target.reset();
}

// Logout
function logout() {
    localStorage.removeItem('authToken');
    authToken = null;
    currentUser = null;
    
    // Reset UI
    window.location.reload();
}

// Load dashboard
async function loadDashboard() {
    if (!authToken) {
        showLoginModal();
        return;
    }
    
    try {
        // Fetch accounts
        const accountsResponse = await fetch(`${API_BASE_URL}/api/accounts/`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        if (!accountsResponse.ok) {
            throw new Error('Failed to fetch accounts');
        }
        
        const accountsData = await accountsResponse.json();
        const accounts = accountsData.results || accountsData;
        
        // Create dashboard HTML
        const dashboardHTML = createDashboardHTML(accounts);
        
        // Hide main content and show dashboard
        document.querySelector('.hero').style.display = 'none';
        document.querySelector('.services').style.display = 'none';
        document.querySelector('.features').style.display = 'none';
        document.querySelector('.dashboard-preview').style.display = 'none';
        document.querySelector('.cta-section').style.display = 'none';
        document.querySelector('.contact').style.display = 'none';
        
        const dashboardSection = document.getElementById('dashboardSection');
        dashboardSection.innerHTML = dashboardHTML;
        dashboardSection.style.display = 'block';
        
        // Load transactions for first account
        if (accounts.length > 0) {
            loadTransactions(accounts[0].id);
        }
        
    } catch (error) {
        console.error('Dashboard error:', error);
        showNotification('Failed to load dashboard', 'error');
    }
}

// Create dashboard HTML
function createDashboardHTML(accounts) {
    const totalBalance = accounts.reduce((sum, acc) => sum + parseFloat(acc.balance), 0);
    
    return `
        <div class="dashboard" style="padding: 120px 0 80px; min-height: 100vh; background: #f1f5f9;">
            <div class="container">
                <div class="dashboard-header" style="margin-bottom: 2rem;">
                    <h1>Welcome back, ${currentUser.user.first_name}!</h1>
                    <p style="color: #64748b;">Manage your accounts and transactions</p>
                </div>
                
                <div class="dashboard-summary" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 1.5rem; margin-bottom: 2rem;">
                    <div class="summary-card" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 2rem; border-radius: 16px; color: white;">
                        <div style="font-size: 0.9rem; opacity: 0.9; margin-bottom: 0.5rem;">Total Balance</div>
                        <div style="font-size: 2.5rem; font-weight: bold;">KES ${totalBalance.toLocaleString('en-US', {minimumFractionDigits: 2, maximumFractionDigits: 2})}</div>
                    </div>
                    <div class="summary-card" style="background: white; padding: 2rem; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.1);">
                        <div style="color: #64748b; margin-bottom: 0.5rem;">Accounts</div>
                        <div style="font-size: 2rem; font-weight: bold; color: #2563eb;">${accounts.length}</div>
                    </div>
                    <div class="summary-card" style="background: white; padding: 2rem; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.1);">
                        <div style="color: #64748b; margin-bottom: 0.5rem;">Active Cards</div>
                        <div style="font-size: 2rem; font-weight: bold; color: #10b981;">0</div>
                    </div>
                </div>
                
                <div class="dashboard-main" style="display: grid; grid-template-columns: 2fr 1fr; gap: 2rem;">
                    <div class="main-content">
                        <div class="accounts-section" style="background: white; padding: 2rem; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.1); margin-bottom: 2rem;">
                            <h2 style="margin-bottom: 1.5rem;">My Accounts</h2>
                            <div class="accounts-list">
                                ${accounts.map(account => `
                                    <div class="account-item" style="padding: 1.5rem; border: 2px solid #e2e8f0; border-radius: 12px; margin-bottom: 1rem; cursor: pointer; transition: all 0.3s;" onmouseover="this.style.borderColor='#2563eb'" onmouseout="this.style.borderColor='#e2e8f0'" onclick="loadTransactions(${account.id})">
                                        <div style="display: flex; justify-content: space-between; align-items: center;">
                                            <div>
                                                <div style="font-weight: 600; font-size: 1.1rem;">${account.account_type} Account</div>
                                                <div style="color: #64748b; font-size: 0.9rem;">${account.account_number}</div>
                                            </div>
                                            <div style="text-align: right;">
                                                <div style="font-size: 1.5rem; font-weight: bold; color: #2563eb;">${account.currency} ${parseFloat(account.balance).toLocaleString('en-US', {minimumFractionDigits: 2, maximumFractionDigits: 2})}</div>
                                                <div style="color: #64748b; font-size: 0.8rem;">${account.status}</div>
                                            </div>
                                        </div>
                                    </div>
                                `).join('')}
                            </div>
                            ${accounts.length === 0 ? '<p style="text-align: center; color: #64748b; padding: 2rem;">No accounts found. <button class="btn btn-primary" onclick="showCreateAccountModal()">Create Account</button></p>' : ''}
                        </div>
                        
                        <div class="transactions-section" style="background: white; padding: 2rem; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.1);">
                            <h2 style="margin-bottom: 1.5rem;">Recent Transactions</h2>
                            <div id="transactionsList">
                                <p style="text-align: center; color: #64748b; padding: 2rem;">Select an account to view transactions</p>
                            </div>
                        </div>
                    </div>
                    
                    <div class="sidebar">
                        <div class="quick-actions" style="background: white; padding: 2rem; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.1); margin-bottom: 2rem;">
                            <h3 style="margin-bottom: 1.5rem;">Quick Actions</h3>
                            <button class="btn btn-primary btn-block" style="margin-bottom: 1rem;" onclick="showDepositModal()">
                                <i class="fas fa-plus-circle"></i> Deposit
                            </button>
                            <button class="btn btn-outline btn-block" style="margin-bottom: 1rem;" onclick="showWithdrawModal()">
                                <i class="fas fa-minus-circle"></i> Withdraw
                            </button>
                            <button class="btn btn-outline btn-block" style="margin-bottom: 1rem;" onclick="showTransferModal()">
                                <i class="fas fa-exchange-alt"></i> Transfer
                            </button>
                            <button class="btn btn-outline btn-block" onclick="showLoanModal()">
                                <i class="fas fa-hand-holding-usd"></i> Apply for Loan
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

// Load transactions for an account
async function loadTransactions(accountId) {
    try {
        const response = await fetch(`${API_BASE_URL}/api/accounts/${accountId}/statement/`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        if (response.ok) {
            const transactions = await response.json();
            displayTransactions(transactions);
        }
    } catch (error) {
        console.error('Failed to load transactions:', error);
    }
}

// Display transactions
function displayTransactions(transactions) {
    const container = document.getElementById('transactionsList');
    
    if (transactions.length === 0) {
        container.innerHTML = '<p style="text-align: center; color: #64748b; padding: 2rem;">No transactions found</p>';
        return;
    }
    
    container.innerHTML = transactions.map(txn => `
        <div style="padding: 1rem; border-bottom: 1px solid #e2e8f0;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <div style="font-weight: 600;">${txn.transaction_type}</div>
                    <div style="color: #64748b; font-size: 0.9rem;">${new Date(txn.created_at).toLocaleDateString()}</div>
                    <div style="color: #64748b; font-size: 0.85rem;">${txn.description}</div>
                </div>
                <div style="text-align: right;">
                    <div style="font-size: 1.2rem; font-weight: bold; color: ${txn.transaction_type === 'DEPOSIT' ? '#10b981' : '#ef4444'};">
                        ${txn.transaction_type === 'DEPOSIT' ? '+' : '-'}${txn.amount}
                    </div>
                    <div style="color: #64748b; font-size: 0.85rem;">Balance: ${txn.balance_after}</div>
                </div>
            </div>
        </div>
    `).join('');
}

// Notification function
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        padding: 1rem 1.5rem;
        background: ${type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#2563eb'};
        color: white;
        border-radius: 8px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.2);
        z-index: 3000;
        animation: slideIn 0.3s ease;
    `;
    notification.textContent = message;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Scroll to section
function scrollToSection(sectionId) {
    const section = document.getElementById(sectionId);
    if (section) {
        section.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

// Placeholder functions for modals (to be implemented)
function showAccountModal(type) {
    showNotification(`Opening ${type} account form...`, 'info');
}

function showLoanModal() {
    showNotification('Loan application form coming soon...', 'info');
}

function showTransferModal() {
    showNotification('Transfer form coming soon...', 'info');
}

function showMobileAppInfo() {
    showNotification('Mobile app download links coming soon...', 'info');
}

function showDepositModal() {
    showNotification('Deposit form coming soon...', 'info');
}

function showWithdrawModal() {
    showNotification('Withdrawal form coming soon...', 'info');
}

function showCreateAccountModal() {
    showNotification('Create account form coming soon...', 'info');
}
