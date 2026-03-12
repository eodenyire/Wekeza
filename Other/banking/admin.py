from django.contrib import admin
from .models import Customer, Account, Transaction, Loan, Card, LoanPayment


@admin.register(Customer)
class CustomerAdmin(admin.ModelAdmin):
    list_display = ['customer_id', 'user', 'phone_number', 'is_verified', 'created_at']
    list_filter = ['is_verified', 'created_at']
    search_fields = ['customer_id', 'user__username', 'user__email', 'phone_number', 'id_number']
    readonly_fields = ['customer_id', 'created_at', 'updated_at']


@admin.register(Account)
class AccountAdmin(admin.ModelAdmin):
    list_display = ['account_number', 'customer', 'account_type', 'balance', 'currency', 'status', 'created_at']
    list_filter = ['account_type', 'currency', 'status', 'created_at']
    search_fields = ['account_number', 'customer__customer_id', 'customer__user__username']
    readonly_fields = ['account_number', 'created_at', 'updated_at']


@admin.register(Transaction)
class TransactionAdmin(admin.ModelAdmin):
    list_display = ['transaction_id', 'account', 'transaction_type', 'amount', 'status', 'created_at']
    list_filter = ['transaction_type', 'status', 'created_at']
    search_fields = ['transaction_id', 'account__account_number', 'reference_number']
    readonly_fields = ['transaction_id', 'created_at']
    date_hierarchy = 'created_at'


@admin.register(Loan)
class LoanAdmin(admin.ModelAdmin):
    list_display = ['loan_id', 'customer', 'loan_type', 'principal_amount', 'outstanding_balance', 'status', 'application_date']
    list_filter = ['loan_type', 'status', 'application_date']
    search_fields = ['loan_id', 'customer__customer_id', 'customer__user__username']
    readonly_fields = ['loan_id', 'application_date']
    date_hierarchy = 'application_date'


@admin.register(Card)
class CardAdmin(admin.ModelAdmin):
    list_display = ['card_number', 'account', 'card_type', 'card_holder_name', 'status', 'expiry_date']
    list_filter = ['card_type', 'status', 'issued_date']
    search_fields = ['card_number', 'card_holder_name', 'account__account_number']
    readonly_fields = ['issued_date']


@admin.register(LoanPayment)
class LoanPaymentAdmin(admin.ModelAdmin):
    list_display = ['payment_id', 'loan', 'amount', 'principal_paid', 'interest_paid', 'payment_date']
    list_filter = ['payment_date']
    search_fields = ['payment_id', 'loan__loan_id']
    readonly_fields = ['payment_id', 'payment_date']
    date_hierarchy = 'payment_date'
