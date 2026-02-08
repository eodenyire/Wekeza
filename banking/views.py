from django.shortcuts import render
from rest_framework import viewsets, status, permissions
from rest_framework.decorators import action, api_view, permission_classes
from rest_framework.response import Response
from rest_framework.permissions import IsAuthenticated, AllowAny
from django.contrib.auth.models import User
from django.utils import timezone
from decimal import Decimal
import random
import string

from .models import Customer, Account, Transaction, Loan, Card, LoanPayment
from .serializers import (
    CustomerSerializer, AccountSerializer, TransactionSerializer,
    LoanSerializer, CardSerializer, LoanPaymentSerializer, RegisterSerializer
)


class CustomerViewSet(viewsets.ModelViewSet):
    queryset = Customer.objects.all()
    serializer_class = CustomerSerializer
    permission_classes = [IsAuthenticated]
    
    def get_queryset(self):
        if self.request.user.is_staff:
            return Customer.objects.all()
        return Customer.objects.filter(user=self.request.user)


class AccountViewSet(viewsets.ModelViewSet):
    queryset = Account.objects.all()
    serializer_class = AccountSerializer
    permission_classes = [IsAuthenticated]
    
    def get_queryset(self):
        if self.request.user.is_staff:
            return Account.objects.all()
        try:
            customer = self.request.user.customer_profile
            return Account.objects.filter(customer=customer)
        except:
            return Account.objects.none()
    
    @action(detail=True, methods=['get'])
    def balance(self, request, pk=None):
        account = self.get_object()
        return Response({
            'account_number': account.account_number,
            'balance': account.balance,
            'currency': account.currency,
            'status': account.status
        })
    
    @action(detail=True, methods=['get'])
    def statement(self, request, pk=None):
        account = self.get_object()
        transactions = account.transactions.all()[:20]
        serializer = TransactionSerializer(transactions, many=True)
        return Response(serializer.data)


class TransactionViewSet(viewsets.ModelViewSet):
    queryset = Transaction.objects.all()
    serializer_class = TransactionSerializer
    permission_classes = [IsAuthenticated]
    
    def get_queryset(self):
        if self.request.user.is_staff:
            return Transaction.objects.all()
        try:
            customer = self.request.user.customer_profile
            accounts = customer.accounts.all()
            return Transaction.objects.filter(account__in=accounts)
        except:
            return Transaction.objects.none()
    
    @action(detail=False, methods=['post'])
    def deposit(self, request):
        account_number = request.data.get('account_number')
        amount = Decimal(request.data.get('amount', 0))
        description = request.data.get('description', 'Deposit')
        
        try:
            account = Account.objects.get(account_number=account_number)
            
            if account.status != 'ACTIVE':
                return Response({'error': 'Account is not active'}, status=status.HTTP_400_BAD_REQUEST)
            
            if amount <= 0:
                return Response({'error': 'Amount must be positive'}, status=status.HTTP_400_BAD_REQUEST)
            
            # Create transaction
            transaction_id = f"TXN{''.join(random.choices(string.ascii_uppercase + string.digits, k=10))}"
            balance_before = account.balance
            balance_after = balance_before + amount
            
            transaction = Transaction.objects.create(
                transaction_id=transaction_id,
                account=account,
                transaction_type='DEPOSIT',
                amount=amount,
                balance_before=balance_before,
                balance_after=balance_after,
                description=description,
                status='COMPLETED',
                processed_by=request.user
            )
            
            # Update account balance
            account.balance = balance_after
            account.save()
            
            serializer = TransactionSerializer(transaction)
            return Response(serializer.data, status=status.HTTP_201_CREATED)
            
        except Account.DoesNotExist:
            return Response({'error': 'Account not found'}, status=status.HTTP_404_NOT_FOUND)
        except Exception as e:
            return Response({'error': str(e)}, status=status.HTTP_400_BAD_REQUEST)
    
    @action(detail=False, methods=['post'])
    def withdraw(self, request):
        account_number = request.data.get('account_number')
        amount = Decimal(request.data.get('amount', 0))
        description = request.data.get('description', 'Withdrawal')
        
        try:
            account = Account.objects.get(account_number=account_number)
            
            if account.status != 'ACTIVE':
                return Response({'error': 'Account is not active'}, status=status.HTTP_400_BAD_REQUEST)
            
            if amount <= 0:
                return Response({'error': 'Amount must be positive'}, status=status.HTTP_400_BAD_REQUEST)
            
            if account.balance < amount:
                return Response({'error': 'Insufficient balance'}, status=status.HTTP_400_BAD_REQUEST)
            
            # Create transaction
            transaction_id = f"TXN{''.join(random.choices(string.ascii_uppercase + string.digits, k=10))}"
            balance_before = account.balance
            balance_after = balance_before - amount
            
            transaction = Transaction.objects.create(
                transaction_id=transaction_id,
                account=account,
                transaction_type='WITHDRAWAL',
                amount=amount,
                balance_before=balance_before,
                balance_after=balance_after,
                description=description,
                status='COMPLETED',
                processed_by=request.user
            )
            
            # Update account balance
            account.balance = balance_after
            account.save()
            
            serializer = TransactionSerializer(transaction)
            return Response(serializer.data, status=status.HTTP_201_CREATED)
            
        except Account.DoesNotExist:
            return Response({'error': 'Account not found'}, status=status.HTTP_404_NOT_FOUND)
        except Exception as e:
            return Response({'error': str(e)}, status=status.HTTP_400_BAD_REQUEST)
    
    @action(detail=False, methods=['post'])
    def transfer(self, request):
        from_account_number = request.data.get('from_account')
        to_account_number = request.data.get('to_account')
        amount = Decimal(request.data.get('amount', 0))
        description = request.data.get('description', 'Transfer')
        
        try:
            from_account = Account.objects.get(account_number=from_account_number)
            to_account = Account.objects.get(account_number=to_account_number)
            
            if from_account.status != 'ACTIVE' or to_account.status != 'ACTIVE':
                return Response({'error': 'One or both accounts are not active'}, status=status.HTTP_400_BAD_REQUEST)
            
            if amount <= 0:
                return Response({'error': 'Amount must be positive'}, status=status.HTTP_400_BAD_REQUEST)
            
            if from_account.balance < amount:
                return Response({'error': 'Insufficient balance'}, status=status.HTTP_400_BAD_REQUEST)
            
            # Debit from source account
            transaction_id_debit = f"TXN{''.join(random.choices(string.ascii_uppercase + string.digits, k=10))}"
            from_balance_before = from_account.balance
            from_balance_after = from_balance_before - amount
            
            debit_transaction = Transaction.objects.create(
                transaction_id=transaction_id_debit,
                account=from_account,
                transaction_type='TRANSFER',
                amount=amount,
                balance_before=from_balance_before,
                balance_after=from_balance_after,
                description=f"{description} - To {to_account_number}",
                status='COMPLETED',
                processed_by=request.user
            )
            
            from_account.balance = from_balance_after
            from_account.save()
            
            # Credit to destination account
            transaction_id_credit = f"TXN{''.join(random.choices(string.ascii_uppercase + string.digits, k=10))}"
            to_balance_before = to_account.balance
            to_balance_after = to_balance_before + amount
            
            credit_transaction = Transaction.objects.create(
                transaction_id=transaction_id_credit,
                account=to_account,
                transaction_type='TRANSFER',
                amount=amount,
                balance_before=to_balance_before,
                balance_after=to_balance_after,
                description=f"{description} - From {from_account_number}",
                status='COMPLETED',
                processed_by=request.user
            )
            
            to_account.balance = to_balance_after
            to_account.save()
            
            return Response({
                'debit_transaction': TransactionSerializer(debit_transaction).data,
                'credit_transaction': TransactionSerializer(credit_transaction).data
            }, status=status.HTTP_201_CREATED)
            
        except Account.DoesNotExist:
            return Response({'error': 'Account not found'}, status=status.HTTP_404_NOT_FOUND)
        except Exception as e:
            return Response({'error': str(e)}, status=status.HTTP_400_BAD_REQUEST)


class LoanViewSet(viewsets.ModelViewSet):
    queryset = Loan.objects.all()
    serializer_class = LoanSerializer
    permission_classes = [IsAuthenticated]
    
    def get_queryset(self):
        if self.request.user.is_staff:
            return Loan.objects.all()
        try:
            customer = self.request.user.customer_profile
            return Loan.objects.filter(customer=customer)
        except:
            return Loan.objects.none()
    
    @action(detail=True, methods=['post'])
    def approve(self, request, pk=None):
        loan = self.get_object()
        
        if not request.user.is_staff:
            return Response({'error': 'Only staff can approve loans'}, status=status.HTTP_403_FORBIDDEN)
        
        if loan.status != 'PENDING':
            return Response({'error': 'Loan is not in pending status'}, status=status.HTTP_400_BAD_REQUEST)
        
        loan.status = 'APPROVED'
        loan.approval_date = timezone.now()
        loan.approved_by = request.user
        loan.outstanding_balance = loan.principal_amount
        
        # Calculate monthly payment (simple calculation)
        monthly_rate = loan.interest_rate / 100 / 12
        if monthly_rate > 0:
            loan.monthly_payment = (loan.principal_amount * monthly_rate * (1 + monthly_rate) ** loan.term_months) / \
                                   ((1 + monthly_rate) ** loan.term_months - 1)
        else:
            loan.monthly_payment = loan.principal_amount / loan.term_months
        
        loan.save()
        
        serializer = LoanSerializer(loan)
        return Response(serializer.data)
    
    @action(detail=True, methods=['post'])
    def disburse(self, request, pk=None):
        loan = self.get_object()
        
        if not request.user.is_staff:
            return Response({'error': 'Only staff can disburse loans'}, status=status.HTTP_403_FORBIDDEN)
        
        if loan.status != 'APPROVED':
            return Response({'error': 'Loan must be approved first'}, status=status.HTTP_400_BAD_REQUEST)
        
        # Disburse to account
        transaction_id = f"TXN{''.join(random.choices(string.ascii_uppercase + string.digits, k=10))}"
        balance_before = loan.account.balance
        balance_after = balance_before + loan.principal_amount
        
        transaction = Transaction.objects.create(
            transaction_id=transaction_id,
            account=loan.account,
            transaction_type='LOAN_DISBURSEMENT',
            amount=loan.principal_amount,
            balance_before=balance_before,
            balance_after=balance_after,
            description=f"Loan disbursement - {loan.loan_id}",
            status='COMPLETED',
            processed_by=request.user
        )
        
        loan.account.balance = balance_after
        loan.account.save()
        
        loan.status = 'DISBURSED'
        loan.disbursement_date = timezone.now()
        loan.save()
        
        return Response({
            'loan': LoanSerializer(loan).data,
            'transaction': TransactionSerializer(transaction).data
        })


class CardViewSet(viewsets.ModelViewSet):
    queryset = Card.objects.all()
    serializer_class = CardSerializer
    permission_classes = [IsAuthenticated]
    
    def get_queryset(self):
        if self.request.user.is_staff:
            return Card.objects.all()
        try:
            customer = self.request.user.customer_profile
            accounts = customer.accounts.all()
            return Card.objects.filter(account__in=accounts)
        except:
            return Card.objects.none()


class LoanPaymentViewSet(viewsets.ModelViewSet):
    queryset = LoanPayment.objects.all()
    serializer_class = LoanPaymentSerializer
    permission_classes = [IsAuthenticated]
    
    def get_queryset(self):
        if self.request.user.is_staff:
            return LoanPayment.objects.all()
        try:
            customer = self.request.user.customer_profile
            return LoanPayment.objects.filter(loan__customer=customer)
        except:
            return LoanPayment.objects.none()


@api_view(['POST'])
@permission_classes([AllowAny])
def register(request):
    serializer = RegisterSerializer(data=request.data)
    if serializer.is_valid():
        user = serializer.save()
        return Response({
            'message': 'User registered successfully',
            'username': user.username,
            'email': user.email
        }, status=status.HTTP_201_CREATED)
    return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)


def index(request):
    """Main landing page"""
    return render(request, 'banking/index.html')
