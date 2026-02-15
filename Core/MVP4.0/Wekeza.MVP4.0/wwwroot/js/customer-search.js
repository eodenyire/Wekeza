// Customer Search Functionality
// This script provides customer search capabilities across all teller pages

// Combined customer database with all information
const customerDatabase = {
    // Original customers
    '1001234567': { 
        name: 'John Smith', 
        idNumber: 'ID123456789',
        phone: '+254712345678',
        email: 'john.smith@email.com',
        balance: 2500.00, 
        available: 2500.00,
        type: 'Savings',
        status: 'Active',
        transactions: [
            { date: '2026-01-22', description: 'ATM Withdrawal', type: 'Debit', amount: -200.00, balance: 2500.00 },
            { date: '2026-01-21', description: 'Direct Deposit', type: 'Credit', amount: 1500.00, balance: 2700.00 }
        ]
    },
    '1001234568': { 
        name: 'Jane Doe', 
        idNumber: 'ID987654321',
        phone: '+254723456789',
        email: 'jane.doe@email.com',
        balance: 5800.00, 
        available: 5300.00,
        type: 'Checking',
        status: 'Active',
        transactions: [
            { date: '2026-01-22', description: 'Salary Deposit', type: 'Credit', amount: 3000.00, balance: 5800.00 },
            { date: '2026-01-21', description: 'Rent Payment', type: 'Debit', amount: -1200.00, balance: 2800.00 }
        ]
    },
    '1001234569': { 
        name: 'Mike Johnson', 
        idNumber: 'ID456789123',
        phone: '+254734567890',
        email: 'mike.johnson@email.com',
        balance: 1200.00, 
        available: 1200.00,
        type: 'Savings',
        status: 'Active',
        transactions: [
            { date: '2026-01-22', description: 'Interest Payment', type: 'Credit', amount: 12.50, balance: 1200.00 },
            { date: '2026-01-21', description: 'Transfer Out', type: 'Debit', amount: -500.00, balance: 1187.50 }
        ]
    },
    '1001234570': { 
        name: 'Sarah Wilson', 
        idNumber: 'ID789123456',
        phone: '+254745678901',
        email: 'sarah.wilson@email.com',
        balance: 3400.00, 
        available: 3400.00,
        type: 'Business',
        status: 'Active',
        transactions: [
            { date: '2026-01-22', description: 'Client Payment', type: 'Credit', amount: 2500.00, balance: 3400.00 },
            { date: '2026-01-21', description: 'Office Supplies', type: 'Debit', amount: -150.00, balance: 900.00 }
        ]
    },
    '1001234571': { 
        name: 'David Brown', 
        idNumber: 'ID321654987',
        phone: '+254756789012',
        email: 'david.brown@email.com',
        balance: 750.00, 
        available: 750.00,
        type: 'Student',
        status: 'Active',
        transactions: [
            { date: '2026-01-22', description: 'Part-time Job', type: 'Credit', amount: 400.00, balance: 750.00 },
            { date: '2026-01-21', description: 'Textbooks', type: 'Debit', amount: -120.00, balance: 350.00 }
        ]
    },
    '1001234572': { 
        name: 'Lisa Davis', 
        idNumber: 'ID654987321',
        phone: '+254767890123',
        email: 'lisa.davis@email.com',
        balance: 4200.00, 
        available: 4200.00,
        type: 'Checking',
        status: 'Active',
        transactions: [
            { date: '2026-01-22', description: 'Freelance Payment', type: 'Credit', amount: 800.00, balance: 4200.00 },
            { date: '2026-01-21', description: 'Credit Card Payment', type: 'Debit', amount: -600.00, balance: 3400.00 }
        ]
    }
};

// Load newly created accounts from localStorage
function loadNewCustomers() {
    try {
        const newAccountsDetailed = JSON.parse(localStorage.getItem('newAccountsDetailed') || '{}');
        Object.assign(customerDatabase, newAccountsDetailed);
    } catch (e) {
        console.warn('Error loading new customers from localStorage:', e);
    }
}

// Initialize customer database
loadNewCustomers();

// Search customers by name, ID number, or account number
function searchCustomers(query) {
    if (!query || query.length < 2) return [];
    
    const searchTerm = query.toLowerCase().trim();
    const results = [];
    
    Object.keys(customerDatabase).forEach(accountNumber => {
        const customer = customerDatabase[accountNumber];
        
        // Search by account number
        if (accountNumber.toLowerCase().includes(searchTerm)) {
            results.push({
                accountNumber: accountNumber,
                customer: customer,
                matchType: 'Account Number'
            });
        }
        // Search by name
        else if (customer.name.toLowerCase().includes(searchTerm)) {
            results.push({
                accountNumber: accountNumber,
                customer: customer,
                matchType: 'Name'
            });
        }
        // Search by ID number
        else if (customer.idNumber && customer.idNumber.toLowerCase().includes(searchTerm)) {
            results.push({
                accountNumber: accountNumber,
                customer: customer,
                matchType: 'ID Number'
            });
        }
        // Search by phone
        else if (customer.phone && customer.phone.includes(searchTerm)) {
            results.push({
                accountNumber: accountNumber,
                customer: customer,
                matchType: 'Phone'
            });
        }
    });
    
    return results.slice(0, 10); // Limit to 10 results
}

// Create customer search modal HTML
function createCustomerSearchModal() {
    const modalHTML = `
        <div class="modal fade" id="customerSearchModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="fas fa-search"></i> Customer Search
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="customerSearchInput" class="form-label">Search by Name, ID Number, Account Number, or Phone</label>
                            <input type="text" class="form-control" id="customerSearchInput" placeholder="Enter search term..." autocomplete="off">
                            <small class="form-text text-muted">Type at least 2 characters to search</small>
                        </div>
                        
                        <div id="searchResults" class="mt-3" style="display: none;">
                            <h6>Search Results:</h6>
                            <div class="list-group" id="searchResultsList">
                            </div>
                        </div>
                        
                        <div id="noResults" class="mt-3 text-muted text-center" style="display: none;">
                            <i class="fas fa-search"></i><br>
                            No customers found matching your search.
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Add modal to page if it doesn't exist
    if (!document.getElementById('customerSearchModal')) {
        document.body.insertAdjacentHTML('beforeend', modalHTML);
    }
}

// Initialize customer search functionality
function initializeCustomerSearch() {
    // Create modal
    createCustomerSearchModal();
    
    // Add search input event listener
    const searchInput = document.getElementById('customerSearchInput');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const query = this.value;
            performSearch(query);
        });
        
        // Clear results when modal is closed
        const modal = document.getElementById('customerSearchModal');
        modal.addEventListener('hidden.bs.modal', function() {
            searchInput.value = '';
            document.getElementById('searchResults').style.display = 'none';
            document.getElementById('noResults').style.display = 'none';
        });
    }
}

// Perform search and display results
function performSearch(query) {
    const results = searchCustomers(query);
    const searchResults = document.getElementById('searchResults');
    const noResults = document.getElementById('noResults');
    const resultsList = document.getElementById('searchResultsList');
    
    if (query.length < 2) {
        searchResults.style.display = 'none';
        noResults.style.display = 'none';
        return;
    }
    
    if (results.length === 0) {
        searchResults.style.display = 'none';
        noResults.style.display = 'block';
        return;
    }
    
    // Display results
    noResults.style.display = 'none';
    searchResults.style.display = 'block';
    
    resultsList.innerHTML = '';
    results.forEach(result => {
        const customer = result.customer;
        const listItem = document.createElement('div');
        listItem.className = 'list-group-item list-group-item-action';
        listItem.style.cursor = 'pointer';
        
        listItem.innerHTML = `
            <div class="d-flex w-100 justify-content-between">
                <h6 class="mb-1">${customer.name}</h6>
                <small class="text-muted">Match: ${result.matchType}</small>
            </div>
            <p class="mb-1">
                <strong>Account:</strong> ${result.accountNumber} | 
                <strong>Type:</strong> ${customer.type} | 
                <strong>Balance:</strong> $${customer.balance.toFixed(2)}
            </p>
            <small>
                <strong>ID:</strong> ${customer.idNumber || 'N/A'} | 
                <strong>Phone:</strong> ${customer.phone || 'N/A'}
            </small>
        `;
        
        listItem.addEventListener('click', function() {
            selectCustomer(result.accountNumber, customer);
        });
        
        resultsList.appendChild(listItem);
    });
}

// Handle customer selection
function selectCustomer(accountNumber, customer) {
    // Fill in account number field if it exists
    const accountNumberField = document.getElementById('accountNumber') || 
                              document.getElementById('fromAccount') || 
                              document.getElementById('toAccount');
    
    if (accountNumberField) {
        accountNumberField.value = accountNumber;
        
        // Trigger the blur event to populate other fields
        const event = new Event('blur', { bubbles: true });
        accountNumberField.dispatchEvent(event);
        
        // Also trigger change event for some forms
        const changeEvent = new Event('change', { bubbles: true });
        accountNumberField.dispatchEvent(changeEvent);
    }
    
    // Close modal
    const modal = bootstrap.Modal.getInstance(document.getElementById('customerSearchModal'));
    if (modal) {
        modal.hide();
    }
    
    // Show success message
    showSearchAlert('success', `Customer selected: ${customer.name} (${accountNumber})`);
}

// Show alert for search actions
function showSearchAlert(type, message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : 'info-circle'}"></i> ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    const container = document.querySelector('.page-header') || document.body.firstChild;
    container.parentNode.insertBefore(alertDiv, container.nextSibling);
    
    // Auto-dismiss after 3 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 3000);
}

// Add search button to forms
function addCustomerSearchButton() {
    // Find account number input fields
    const accountFields = document.querySelectorAll('input[id*="account"], input[id*="Account"]');
    
    accountFields.forEach(field => {
        // Skip if search button already exists
        if (field.parentNode.querySelector('.customer-search-btn')) return;
        
        // Create search button
        const searchBtn = document.createElement('button');
        searchBtn.type = 'button';
        searchBtn.className = 'btn btn-outline-primary customer-search-btn';
        searchBtn.innerHTML = '<i class="fas fa-search"></i>';
        searchBtn.title = 'Search Customer';
        searchBtn.style.marginLeft = '5px';
        
        searchBtn.addEventListener('click', function() {
            const modal = new bootstrap.Modal(document.getElementById('customerSearchModal'));
            modal.show();
        });
        
        // Add button after the input field
        field.parentNode.insertBefore(searchBtn, field.nextSibling);
    });
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Wait a bit for other scripts to load
    setTimeout(() => {
        initializeCustomerSearch();
        addCustomerSearchButton();
        
        // Reload customers in case new ones were added
        loadNewCustomers();
    }, 500);
});

// Export functions for use in other scripts
window.CustomerSearch = {
    searchCustomers,
    selectCustomer,
    loadNewCustomers,
    customerDatabase
};