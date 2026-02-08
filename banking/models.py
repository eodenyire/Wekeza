from django.db import models
from django.contrib.auth.models import User
from django.core.validators import MinValueValidator
from decimal import Decimal


class Customer(models.Model):
    """Customer model for bank customers"""
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='customer_profile')
    customer_id = models.CharField(max_length=20, unique=True)
    phone_number = models.CharField(max_length=15)
    address = models.TextField()
    date_of_birth = models.DateField()
    id_number = models.CharField(max_length=20, unique=True)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    is_verified = models.BooleanField(default=False)
    
    class Meta:
        db_table = 'customers'
        verbose_name = 'Customer'
        verbose_name_plural = 'Customers'
    
    def __str__(self):
        return f"{self.customer_id} - {self.user.get_full_name()}"


class Account(models.Model):
    """Bank account model"""
    ACCOUNT_TYPES = [
        ('SAVINGS', 'Savings Account'),
        ('CHECKING', 'Checking Account'),
        ('BUSINESS', 'Business Account'),
        ('FIXED_DEPOSIT', 'Fixed Deposit'),
    ]
    
    CURRENCY_CHOICES = [
        ('KES', 'Kenyan Shilling'),
        ('USD', 'US Dollar'),
        ('EUR', 'Euro'),
        ('GBP', 'British Pound'),
    ]
    
    STATUS_CHOICES = [
        ('ACTIVE', 'Active'),
        ('FROZEN', 'Frozen'),
        ('CLOSED', 'Closed'),
    ]
    
    customer = models.ForeignKey(Customer, on_delete=models.CASCADE, related_name='accounts')
    account_number = models.CharField(max_length=20, unique=True)
    account_type = models.CharField(max_length=20, choices=ACCOUNT_TYPES)
    currency = models.CharField(max_length=3, choices=CURRENCY_CHOICES, default='KES')
    balance = models.DecimalField(max_digits=15, decimal_places=2, default=0, validators=[MinValueValidator(Decimal('0.00'))])
    status = models.CharField(max_length=10, choices=STATUS_CHOICES, default='ACTIVE')
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    interest_rate = models.DecimalField(max_digits=5, decimal_places=2, default=0)
    
    class Meta:
        db_table = 'accounts'
        verbose_name = 'Account'
        verbose_name_plural = 'Accounts'
    
    def __str__(self):
        return f"{self.account_number} - {self.customer.user.get_full_name()} - {self.balance} {self.currency}"


class Transaction(models.Model):
    """Transaction model for all banking transactions"""
    TRANSACTION_TYPES = [
        ('DEPOSIT', 'Deposit'),
        ('WITHDRAWAL', 'Withdrawal'),
        ('TRANSFER', 'Transfer'),
        ('LOAN_DISBURSEMENT', 'Loan Disbursement'),
        ('LOAN_REPAYMENT', 'Loan Repayment'),
        ('CARD_PAYMENT', 'Card Payment'),
        ('INTEREST', 'Interest'),
        ('FEE', 'Fee'),
    ]
    
    STATUS_CHOICES = [
        ('PENDING', 'Pending'),
        ('COMPLETED', 'Completed'),
        ('FAILED', 'Failed'),
        ('REVERSED', 'Reversed'),
    ]
    
    transaction_id = models.CharField(max_length=50, unique=True)
    account = models.ForeignKey(Account, on_delete=models.CASCADE, related_name='transactions')
    transaction_type = models.CharField(max_length=20, choices=TRANSACTION_TYPES)
    amount = models.DecimalField(max_digits=15, decimal_places=2, validators=[MinValueValidator(Decimal('0.01'))])
    balance_before = models.DecimalField(max_digits=15, decimal_places=2)
    balance_after = models.DecimalField(max_digits=15, decimal_places=2)
    description = models.TextField()
    status = models.CharField(max_length=10, choices=STATUS_CHOICES, default='PENDING')
    created_at = models.DateTimeField(auto_now_add=True)
    processed_by = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, blank=True)
    reference_number = models.CharField(max_length=50, blank=True, null=True)
    
    class Meta:
        db_table = 'transactions'
        verbose_name = 'Transaction'
        verbose_name_plural = 'Transactions'
        ordering = ['-created_at']
    
    def __str__(self):
        return f"{self.transaction_id} - {self.transaction_type} - {self.amount}"


class Loan(models.Model):
    """Loan model for customer loans"""
    LOAN_TYPES = [
        ('PERSONAL', 'Personal Loan'),
        ('BUSINESS', 'Business Loan'),
        ('MORTGAGE', 'Mortgage'),
        ('AUTO', 'Auto Loan'),
        ('EDUCATION', 'Education Loan'),
    ]
    
    STATUS_CHOICES = [
        ('PENDING', 'Pending Approval'),
        ('APPROVED', 'Approved'),
        ('DISBURSED', 'Disbursed'),
        ('ACTIVE', 'Active'),
        ('PAID', 'Fully Paid'),
        ('DEFAULTED', 'Defaulted'),
        ('REJECTED', 'Rejected'),
    ]
    
    customer = models.ForeignKey(Customer, on_delete=models.CASCADE, related_name='loans')
    account = models.ForeignKey(Account, on_delete=models.CASCADE, related_name='loans')
    loan_id = models.CharField(max_length=20, unique=True)
    loan_type = models.CharField(max_length=20, choices=LOAN_TYPES)
    principal_amount = models.DecimalField(max_digits=15, decimal_places=2, validators=[MinValueValidator(Decimal('1.00'))])
    interest_rate = models.DecimalField(max_digits=5, decimal_places=2, validators=[MinValueValidator(Decimal('0.00'))])
    term_months = models.IntegerField(validators=[MinValueValidator(1)])
    monthly_payment = models.DecimalField(max_digits=15, decimal_places=2, default=0)
    outstanding_balance = models.DecimalField(max_digits=15, decimal_places=2, default=0)
    status = models.CharField(max_length=20, choices=STATUS_CHOICES, default='PENDING')
    application_date = models.DateTimeField(auto_now_add=True)
    approval_date = models.DateTimeField(null=True, blank=True)
    disbursement_date = models.DateTimeField(null=True, blank=True)
    approved_by = models.ForeignKey(User, on_delete=models.SET_NULL, null=True, blank=True, related_name='approved_loans')
    notes = models.TextField(blank=True)
    
    class Meta:
        db_table = 'loans'
        verbose_name = 'Loan'
        verbose_name_plural = 'Loans'
        ordering = ['-application_date']
    
    def __str__(self):
        return f"{self.loan_id} - {self.customer.user.get_full_name()} - {self.principal_amount}"


class Card(models.Model):
    """Card model for debit/credit cards"""
    CARD_TYPES = [
        ('DEBIT', 'Debit Card'),
        ('CREDIT', 'Credit Card'),
    ]
    
    STATUS_CHOICES = [
        ('ACTIVE', 'Active'),
        ('BLOCKED', 'Blocked'),
        ('EXPIRED', 'Expired'),
        ('CANCELLED', 'Cancelled'),
    ]
    
    account = models.ForeignKey(Account, on_delete=models.CASCADE, related_name='cards')
    card_number = models.CharField(max_length=16, unique=True)
    card_type = models.CharField(max_length=10, choices=CARD_TYPES)
    card_holder_name = models.CharField(max_length=100)
    expiry_date = models.DateField()
    cvv = models.CharField(max_length=3)
    daily_limit = models.DecimalField(max_digits=15, decimal_places=2, default=50000)
    status = models.CharField(max_length=10, choices=STATUS_CHOICES, default='ACTIVE')
    issued_date = models.DateTimeField(auto_now_add=True)
    
    class Meta:
        db_table = 'cards'
        verbose_name = 'Card'
        verbose_name_plural = 'Cards'
    
    def __str__(self):
        return f"{self.card_number[-4:]} - {self.card_holder_name}"


class LoanPayment(models.Model):
    """Loan payment model for tracking loan repayments"""
    loan = models.ForeignKey(Loan, on_delete=models.CASCADE, related_name='payments')
    payment_id = models.CharField(max_length=50, unique=True)
    amount = models.DecimalField(max_digits=15, decimal_places=2, validators=[MinValueValidator(Decimal('0.01'))])
    payment_date = models.DateTimeField(auto_now_add=True)
    principal_paid = models.DecimalField(max_digits=15, decimal_places=2)
    interest_paid = models.DecimalField(max_digits=15, decimal_places=2)
    balance_after = models.DecimalField(max_digits=15, decimal_places=2)
    transaction = models.OneToOneField(Transaction, on_delete=models.SET_NULL, null=True, blank=True)
    
    class Meta:
        db_table = 'loan_payments'
        verbose_name = 'Loan Payment'
        verbose_name_plural = 'Loan Payments'
        ordering = ['-payment_date']
    
    def __str__(self):
        return f"{self.payment_id} - {self.loan.loan_id} - {self.amount}"
