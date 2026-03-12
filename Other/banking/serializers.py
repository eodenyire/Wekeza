from rest_framework import serializers
from django.contrib.auth.models import User
from .models import Customer, Account, Transaction, Loan, Card, LoanPayment


class UserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ['id', 'username', 'email', 'first_name', 'last_name']
        read_only_fields = ['id']


class CustomerSerializer(serializers.ModelSerializer):
    user = UserSerializer(read_only=True)
    full_name = serializers.SerializerMethodField()
    
    class Meta:
        model = Customer
        fields = '__all__'
        read_only_fields = ['customer_id', 'created_at', 'updated_at']
    
    def get_full_name(self, obj):
        return obj.user.get_full_name()


class AccountSerializer(serializers.ModelSerializer):
    customer_name = serializers.SerializerMethodField()
    
    class Meta:
        model = Account
        fields = '__all__'
        read_only_fields = ['account_number', 'balance', 'created_at', 'updated_at']
    
    def get_customer_name(self, obj):
        return obj.customer.user.get_full_name()


class TransactionSerializer(serializers.ModelSerializer):
    account_number = serializers.CharField(source='account.account_number', read_only=True)
    
    class Meta:
        model = Transaction
        fields = '__all__'
        read_only_fields = ['transaction_id', 'balance_before', 'balance_after', 'created_at']


class LoanSerializer(serializers.ModelSerializer):
    customer_name = serializers.SerializerMethodField()
    account_number = serializers.CharField(source='account.account_number', read_only=True)
    
    class Meta:
        model = Loan
        fields = '__all__'
        read_only_fields = ['loan_id', 'application_date', 'approval_date', 'disbursement_date']
    
    def get_customer_name(self, obj):
        return obj.customer.user.get_full_name()


class CardSerializer(serializers.ModelSerializer):
    account_number = serializers.CharField(source='account.account_number', read_only=True)
    masked_card_number = serializers.SerializerMethodField()
    
    class Meta:
        model = Card
        fields = '__all__'
        read_only_fields = ['card_number', 'cvv', 'issued_date']
    
    def get_masked_card_number(self, obj):
        return f"**** **** **** {obj.card_number[-4:]}"


class LoanPaymentSerializer(serializers.ModelSerializer):
    loan_id = serializers.CharField(source='loan.loan_id', read_only=True)
    
    class Meta:
        model = LoanPayment
        fields = '__all__'
        read_only_fields = ['payment_id', 'payment_date']


class RegisterSerializer(serializers.ModelSerializer):
    password = serializers.CharField(write_only=True, required=True, style={'input_type': 'password'})
    password2 = serializers.CharField(write_only=True, required=True, style={'input_type': 'password'})
    phone_number = serializers.CharField(required=True)
    address = serializers.CharField(required=True)
    date_of_birth = serializers.DateField(required=True)
    id_number = serializers.CharField(required=True)
    
    class Meta:
        model = User
        fields = ['username', 'password', 'password2', 'email', 'first_name', 'last_name', 
                  'phone_number', 'address', 'date_of_birth', 'id_number']
    
    def validate(self, attrs):
        if attrs['password'] != attrs['password2']:
            raise serializers.ValidationError({"password": "Password fields didn't match."})
        return attrs
    
    def create(self, validated_data):
        # Extract customer-specific fields
        phone_number = validated_data.pop('phone_number')
        address = validated_data.pop('address')
        date_of_birth = validated_data.pop('date_of_birth')
        id_number = validated_data.pop('id_number')
        validated_data.pop('password2')
        
        # Create user
        user = User.objects.create_user(
            username=validated_data['username'],
            email=validated_data.get('email', ''),
            first_name=validated_data.get('first_name', ''),
            last_name=validated_data.get('last_name', ''),
            password=validated_data['password']
        )
        
        # Create customer profile
        import random
        customer_id = f"CUST{random.randint(100000, 999999)}"
        Customer.objects.create(
            user=user,
            customer_id=customer_id,
            phone_number=phone_number,
            address=address,
            date_of_birth=date_of_birth,
            id_number=id_number
        )
        
        return user
