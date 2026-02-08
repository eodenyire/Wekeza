# Wekeza Mobile - Visual Architecture Guide

This document provides visual representations of the app's architecture and data flow.

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     FLUTTER MOBILE APP                       │
│                    (Dart/Flutter 3.0+)                       │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                  PRESENTATION LAYER                  │    │
│  │                      (Screens)                       │    │
│  ├─────────────────────────────────────────────────────┤    │
│  │  • Login Screen         • Loans Screen              │    │
│  │  • Dashboard Screen     • Cards Screen              │    │
│  │  • Accounts Screen      • Profile Screen            │    │
│  │  • Transactions Screen  • Settings Screen           │    │
│  │  • Transfer Screen      • [More screens...]         │    │
│  └────────────────┬────────────────────────────────────┘    │
│                   │                                          │
│  ┌────────────────▼────────────────────────────────────┐    │
│  │                   SERVICE LAYER                      │    │
│  ├─────────────────────────────────────────────────────┤    │
│  │  • Auth Service       • Loan Service                │    │
│  │  • Account Service    • Card Service                │    │
│  │  • Transaction Service • Payment Service            │    │
│  │  • API Service (HTTP Client)                        │    │
│  │  • Storage Service (Local/Secure)                   │    │
│  └────────────────┬────────────────────────────────────┘    │
│                   │                                          │
│  ┌────────────────▼────────────────────────────────────┐    │
│  │                     DATA LAYER                       │    │
│  ├─────────────────────────────────────────────────────┤    │
│  │  • Models (User, Account, Transaction, etc.)        │    │
│  │  • Local Storage (Shared Preferences)               │    │
│  │  • Secure Storage (Encrypted)                       │    │
│  └────────────────┬────────────────────────────────────┘    │
│                   │                                          │
└───────────────────┼──────────────────────────────────────────┘
                    │
                    │ HTTP/REST API
                    │ JSON Format
                    │ JWT Authentication
                    ▼
┌─────────────────────────────────────────────────────────────┐
│              WEKEZA CORE BANKING SYSTEM                      │
│                    (.NET 8 / C#)                             │
├─────────────────────────────────────────────────────────────┤
│  • Authentication API    • Payment API                       │
│  • Account API          • Card API                           │
│  • Transaction API      • Loan API                           │
│  • [All Banking APIs...]                                     │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
              ┌──────────────────────┐
              │   PostgreSQL DB      │
              │  (Banking Data)      │
              └──────────────────────┘
```

## Authentication Flow

```
┌──────────┐                ┌──────────┐              ┌──────────┐
│  User    │                │  Mobile  │              │ Backend  │
│          │                │   App    │              │   API    │
└────┬─────┘                └────┬─────┘              └────┬─────┘
     │                           │                         │
     │  1. Open App              │                         │
     ├──────────────────────────►│                         │
     │                           │                         │
     │                           │  2. Check Token         │
     │                           ├────────┐                │
     │                           │        │                │
     │                           │◄───────┘                │
     │                           │                         │
     │  If No Token:             │                         │
     │  3. Show Login            │                         │
     │◄──────────────────────────┤                         │
     │                           │                         │
     │  4. Enter Credentials     │                         │
     ├──────────────────────────►│                         │
     │                           │                         │
     │                           │  5. POST /login         │
     │                           ├────────────────────────►│
     │                           │                         │
     │                           │  6. Validate Creds      │
     │                           │                         ├────┐
     │                           │                         │    │
     │                           │                         │◄───┘
     │                           │                         │
     │                           │  7. Return JWT + User   │
     │                           │◄────────────────────────┤
     │                           │                         │
     │                           │  8. Store Token         │
     │                           ├────────┐                │
     │                           │        │                │
     │                           │◄───────┘                │
     │                           │                         │
     │  9. Navigate to Dashboard │                         │
     │◄──────────────────────────┤                         │
     │                           │                         │
```

## API Request Flow

```
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│   Screen    │      │   Service   │      │ API Service │
└──────┬──────┘      └──────┬──────┘      └──────┬──────┘
       │                    │                     │
       │  1. User Action    │                     │
       ├───────────────────►│                     │
       │                    │                     │
       │  2. Show Loading   │                     │
       │◄───────────────────┤                     │
       │                    │                     │
       │                    │  3. Call API Method │
       │                    ├────────────────────►│
       │                    │                     │
       │                    │                     │  4. Get Token
       │                    │                     ├──────────┐
       │                    │                     │          │
       │                    │                     │◄─────────┘
       │                    │                     │
       │                    │                     │  5. Build Request
       │                    │                     │     (Headers, Body)
       │                    │                     ├──────────┐
       │                    │                     │          │
       │                    │                     │◄─────────┘
       │                    │                     │
       │                    │                     │  6. HTTP Request
       │                    │                     │────────────────►
       │                    │                     │                Backend
       │                    │                     │  7. Response
       │                    │                     │◄────────────────
       │                    │                     │
       │                    │                     │  8. Parse JSON
       │                    │                     ├──────────┐
       │                    │                     │          │
       │                    │                     │◄─────────┘
       │                    │                     │
       │                    │  9. Return Data     │
       │                    │◄────────────────────┤
       │                    │                     │
       │  10. Update UI     │                     │
       │◄───────────────────┤                     │
       │                    │                     │
       │  11. Hide Loading  │                     │
       │◄───────────────────┤                     │
       │                    │                     │
```

## Data Flow - Transfer Money

```
User Input                                      Backend Processing
─────────────────────────────────────────────────────────────────

┌───────────────┐
│ Transfer      │
│ Screen        │  1. Enter Details
│               │     • From Account
│ [From: ACC1 ] │     • To Account
│ [To: ACC2   ] │     • Amount
│ [Amount: 1000]│     • Description
│ [Desc: Pay  ] │
│               │
│  [Transfer]   │  2. Click Button
└───────┬───────┘
        │
        │  3. Validate Input
        ▼
┌───────────────┐
│ Validation    │
│               │  • Check amount > 0
│ ✓ Amount OK   │  • Check balance sufficient
│ ✓ Balance OK  │  • Check account valid
│ ✓ Account OK  │
└───────┬───────┘
        │
        │  4. Create Request
        ▼
┌───────────────────────┐
│ Transaction Service   │
│                       │  {
│ transferFunds()       │    "fromAccountId": "...",
│                       │    "toAccountId": "...",
│                       │    "amount": 1000,
│                       │    "description": "Pay"
│                       │  }
└───────┬───────────────┘
        │
        │  5. POST /api/transactions/transfer
        │     Authorization: Bearer <JWT>
        ▼
                         ┌──────────────────────┐
                         │  Backend Validates   │
                         │                      │
                         │  • Check auth token  │
                         │  • Verify from acc   │
                         │  • Verify to acc     │
                         │  • Check balance     │
                         │  • Check limits      │
                         └──────────┬───────────┘
                                    │
                                    │  6. Process Transfer
                                    ▼
                         ┌──────────────────────┐
                         │  Database Update     │
                         │                      │
                         │  • Debit from acc    │
                         │  • Credit to acc     │
                         │  • Create txn record │
                         │  • Update balances   │
                         │  • Post to GL        │
                         └──────────┬───────────┘
                                    │
                                    │  7. Return Result
                                    ▼
┌───────────────────────┐
│ Response              │  {
│                       │    "success": true,
│ Success!              │    "transactionId": "...",
│ Ref: TXN001           │    "referenceNumber": "TXN001",
│                       │    "status": "Completed"
│ [Done]                │  }
└───────────────────────┘
```

## Screen Navigation Map

```
                    ┌──────────────┐
                    │   Splash     │
                    │   Screen     │
                    └──────┬───────┘
                           │
                    Check Token?
                           │
           ┌───────────────┴────────────────┐
           │                                │
         NO│                                │YES
           │                                │
    ┌──────▼───────┐               ┌───────▼────────┐
    │    Login     │               │   Dashboard    │
    │    Screen    │               │    Screen      │
    └──────┬───────┘               └───────┬────────┘
           │                               │
           │ Authenticate                  │
           └───────────────────────────────┘
                                           │
                    ┌──────────────────────┼──────────────────────┐
                    │                      │                      │
             ┌──────▼────────┐     ┌──────▼────────┐     ┌──────▼────────┐
             │   Accounts    │     │ Transactions  │     │   Transfer    │
             │    Screen     │     │    Screen     │     │    Screen     │
             └───────────────┘     └───────────────┘     └───────────────┘
                    │                      │                      │
        ┌───────────┴──────────┐          │              ┌───────▼────────┐
        │                      │          │              │    Success     │
 ┌──────▼────────┐    ┌───────▼──────┐   │              │    Dialog      │
 │   Account     │    │  Statement   │   │              └────────────────┘
 │   Details     │    │    Screen    │   │
 │   Modal       │    │              │   │
 └───────────────┘    └──────────────┘   │
                                          │
             ┌────────────────────────────┴────────────────────────┐
             │                                                     │
      ┌──────▼────────┐                                   ┌───────▼────────┐
      │     Loans     │                                   │     Cards      │
      │    Screen     │                                   │    Screen      │
      └──────┬────────┘                                   └───────┬────────┘
             │                                                    │
      ┌──────▼────────┐                                   ┌──────▼────────┐
      │  Loan Details │                                   │ Card Details  │
      │    Modal      │                                   │    Modal      │
      └───────────────┘                                   └───────────────┘
```

## State Management Flow

```
                    ┌──────────────────┐
                    │   User Action    │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │  Update State    │
                    │  (setState())    │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │  Call Service    │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │   API Request    │
                    └────────┬─────────┘
                             │
                ┌────────────┴────────────┐
                │                         │
        ┌───────▼────────┐       ┌───────▼────────┐
        │    Success     │       │     Error      │
        └───────┬────────┘       └───────┬────────┘
                │                        │
        ┌───────▼────────┐       ┌───────▼────────┐
        │  Update State  │       │  Update State  │
        │  (with data)   │       │  (with error)  │
        └───────┬────────┘       └───────┬────────┘
                │                        │
                └────────────┬───────────┘
                             │
                    ┌────────▼─────────┐
                    │   UI Rebuild     │
                    │  (auto-refresh)  │
                    └──────────────────┘
```

## Security Layers

```
┌─────────────────────────────────────────────────────────┐
│                    USER INTERFACE                        │
│  • Input validation                                      │
│  • Client-side checks                                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  APPLICATION LAYER                       │
│  • JWT token validation                                  │
│  • Session management                                    │
│  • Request encryption                                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                   NETWORK LAYER                          │
│  • HTTPS only                                            │
│  • Certificate pinning                                   │
│  • Request timeout                                       │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                    BACKEND API                           │
│  • Server-side validation                                │
│  • Authorization checks                                  │
│  • Rate limiting                                         │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                     DATABASE                             │
│  • Encrypted data at rest                                │
│  • Transaction logging                                   │
│  • Audit trails                                          │
└─────────────────────────────────────────────────────────┘
```

## File Organization

```
Wekeza Mobile/
│
├── lib/
│   ├── config/
│   │   └── app_config.dart         ← All configuration constants
│   │
│   ├── models/                     ← Data structures
│   │   ├── user_model.dart
│   │   ├── account_model.dart
│   │   ├── transaction_model.dart
│   │   ├── loan_model.dart
│   │   └── card_model.dart
│   │
│   ├── services/                   ← Business logic & API
│   │   ├── api_service.dart        ← Generic HTTP client
│   │   ├── auth_service.dart       ← Authentication
│   │   ├── storage_service.dart    ← Local storage
│   │   ├── account_service.dart    ← Account operations
│   │   ├── transaction_service.dart
│   │   ├── loan_service.dart
│   │   └── card_service.dart
│   │
│   ├── screens/                    ← UI screens
│   │   ├── login_screen.dart
│   │   ├── dashboard_screen.dart
│   │   ├── accounts_screen.dart
│   │   ├── transactions_screen.dart
│   │   ├── transfer_screen.dart
│   │   ├── loans_screen.dart
│   │   └── cards_screen.dart
│   │
│   ├── widgets/                    ← Reusable UI components
│   │   └── [custom widgets]
│   │
│   ├── utils/                      ← Helper functions
│   │   └── format_utils.dart
│   │
│   └── main.dart                   ← App entry point
│
├── android/                        ← Android-specific files
├── ios/                            ← iOS-specific files
├── test/                           ← Unit tests
├── integration_test/               ← Integration tests
│
├── pubspec.yaml                    ← Dependencies
├── .gitignore                      ← Git ignore rules
├── analysis_options.yaml           ← Dart linting
│
└── [Documentation]
    ├── README.md                   ← Main docs
    ├── QUICKSTART.md               ← Setup guide
    ├── INTEGRATION.md              ← API guide
    ├── SUMMARY.md                  ← Project summary
    └── ARCHITECTURE.md             ← This file
```

---

This visual guide helps understand how all components work together in the Wekeza Mobile banking application.
