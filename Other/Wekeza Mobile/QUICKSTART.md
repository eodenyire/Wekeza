# Wekeza Mobile - Quick Start Guide

This guide will help you get the Wekeza Mobile app up and running quickly.

## Prerequisites Checklist

Before starting, make sure you have:

- [ ] Flutter SDK 3.0 or higher installed
- [ ] Dart SDK 3.0 or higher installed
- [ ] Android Studio (for Android development) or Xcode (for iOS development)
- [ ] A code editor (VS Code recommended)
- [ ] Git installed
- [ ] Wekeza Core Banking backend running

## Step-by-Step Setup

### 1. Install Flutter

#### Windows
```bash
# Download Flutter SDK from https://flutter.dev/docs/get-started/install/windows
# Extract to C:\src\flutter
# Add to PATH: C:\src\flutter\bin

# Verify installation
flutter doctor
```

#### macOS
```bash
# Download Flutter SDK from https://flutter.dev/docs/get-started/install/macos
# Extract to ~/development/flutter
# Add to PATH in ~/.zshrc or ~/.bashrc:
export PATH="$PATH:$HOME/development/flutter/bin"

# Verify installation
flutter doctor
```

#### Linux
```bash
# Download Flutter SDK
cd ~
git clone https://github.com/flutter/flutter.git -b stable

# Add to PATH in ~/.bashrc:
export PATH="$PATH:$HOME/flutter/bin"

# Verify installation
flutter doctor
```

### 2. Clone the Repository

```bash
git clone https://github.com/eodenyire/Wekeza.git
cd Wekeza/Wekeza\ Mobile
```

### 3. Install Dependencies

```bash
flutter pub get
```

This will download all the required packages listed in `pubspec.yaml`.

### 4. Configure the API Endpoint

Open `lib/config/app_config.dart` and update the API base URL:

```dart
// For production
static const String apiBaseUrl = 'https://api.wekeza.com/api';

// For local development (adjust as needed)
static const String apiBaseUrl = 'http://localhost:5000/api';

// For Android emulator accessing localhost
static const String apiBaseUrl = 'http://10.0.2.2:5000/api';

// For iOS simulator accessing localhost
static const String apiBaseUrl = 'http://localhost:5000/api';
```

**Important Network Notes:**
- Android emulator: Use `10.0.2.2` to access host machine's localhost
- iOS simulator: Can use `localhost` directly
- Physical devices: Use your computer's IP address (e.g., `http://192.168.1.100:5000/api`)

### 5. Start the Backend

Make sure the Wekeza Core Banking backend is running:

```bash
# Navigate to the Core API directory
cd ../Core/Wekeza.Core.Api

# Run the backend
dotnet run

# The backend should start on http://localhost:5000
```

### 6. Run the Flutter App

#### Option A: Using Command Line

```bash
# List available devices
flutter devices

# Run on connected device/emulator
flutter run

# Run on specific device
flutter run -d <device-id>

# Run in debug mode (default)
flutter run

# Run in release mode
flutter run --release
```

#### Option B: Using VS Code

1. Open the project in VS Code
2. Install the Flutter extension
3. Press F5 or click "Run" > "Start Debugging"
4. Select your target device

#### Option C: Using Android Studio

1. Open the project in Android Studio
2. Wait for Gradle sync to complete
3. Select a device/emulator from the toolbar
4. Click the Run button (green play icon)

### 7. Test the App

Default test credentials (if available):
- **Username**: `admin` or `testuser`
- **Password**: (check with your backend team)

## Troubleshooting

### Common Issues and Solutions

#### Issue: "Flutter command not found"
**Solution:**
```bash
# Verify Flutter is in your PATH
echo $PATH  # Linux/macOS
echo %PATH%  # Windows

# Add Flutter to PATH if missing
export PATH="$PATH:[PATH_TO_FLUTTER_GIT_DIRECTORY]/flutter/bin"
```

#### Issue: "Unable to connect to API"
**Solution:**
1. Check if backend is running: `curl http://localhost:5000/api/health`
2. Verify API URL in `app_config.dart`
3. For Android emulator, use `10.0.2.2` instead of `localhost`
4. Check firewall settings
5. Ensure CORS is configured on backend

#### Issue: "Gradle build failed" (Android)
**Solution:**
```bash
cd android
./gradlew clean
cd ..
flutter clean
flutter pub get
flutter run
```

#### Issue: "CocoaPods error" (iOS)
**Solution:**
```bash
cd ios
pod deintegrate
pod install
cd ..
flutter run
```

#### Issue: "Package version conflicts"
**Solution:**
```bash
flutter clean
flutter pub upgrade
flutter pub get
```

#### Issue: "Certificate verification failed"
**Solution:**
For development only, you can disable SSL verification in `api_service.dart`:
```dart
// Add this for development only
HttpClient httpClient = HttpClient()
  ..badCertificateCallback = (X509Certificate cert, String host, int port) => true;
```

### Debugging Tips

1. **Enable verbose logging:**
   ```bash
   flutter run -v
   ```

2. **Check Flutter doctor:**
   ```bash
   flutter doctor -v
   ```

3. **View device logs:**
   ```bash
   flutter logs
   ```

4. **Hot reload:**
   - Press `r` in terminal
   - Or save file in VS Code/Android Studio

5. **Hot restart:**
   - Press `R` in terminal
   - Or use IDE button

## Testing on Physical Devices

### Android

1. Enable Developer Options on your Android device
2. Enable USB Debugging
3. Connect device via USB
4. Run `flutter devices` to verify connection
5. Run `flutter run`

### iOS

1. Connect iPhone/iPad via USB
2. Trust the computer on the device
3. In Xcode, select your device
4. Run `flutter run`

Note: iOS deployment requires:
- Apple Developer account (for physical devices)
- Properly configured provisioning profiles
- Code signing certificate

## Building for Production

### Android APK
```bash
flutter build apk --release
# Output: build/app/outputs/flutter-apk/app-release.apk
```

### Android App Bundle (for Google Play)
```bash
flutter build appbundle --release
# Output: build/app/outputs/bundle/release/app-release.aab
```

### iOS
```bash
flutter build ios --release
# Then open ios/Runner.xcworkspace in Xcode
# Archive and upload to App Store
```

## Project Structure Overview

```
lib/
├── config/          # Configuration files
├── models/          # Data models
├── services/        # API and business logic
├── screens/         # UI screens
├── widgets/         # Reusable widgets
├── utils/           # Helper functions
└── main.dart        # App entry point
```

## Key Files to Know

- `pubspec.yaml` - Dependencies and assets
- `lib/config/app_config.dart` - App configuration
- `lib/main.dart` - App entry point
- `lib/services/api_service.dart` - HTTP client
- `android/app/build.gradle` - Android config
- `ios/Runner.xcodeproj` - iOS config

## Next Steps

1. ✅ Get the app running
2. ✅ Test login functionality
3. ✅ Explore all screens
4. ✅ Test API integration
5. 📱 Customize branding (colors, logo)
6. 🎨 Add custom icons
7. 🔔 Implement push notifications
8. 🔒 Add biometric authentication
9. 🌍 Add localization
10. 📊 Add analytics

## Getting Help

- **Documentation**: See README.md for detailed docs
- **Issues**: Create an issue on GitHub
- **Email**: support@wekeza.com
- **Phone**: +254-700-123-456

## Useful Commands

```bash
# Check Flutter version
flutter --version

# Update Flutter
flutter upgrade

# Clean build files
flutter clean

# Get dependencies
flutter pub get

# Update dependencies
flutter pub upgrade

# Analyze code
flutter analyze

# Run tests
flutter test

# Format code
flutter format .

# Check for outdated packages
flutter pub outdated
```

## Development Best Practices

1. **Always test on multiple devices**
2. **Use hot reload for faster development**
3. **Run `flutter analyze` before committing**
4. **Write tests for critical functionality**
5. **Use meaningful commit messages**
6. **Keep dependencies up to date**
7. **Follow the Dart style guide**

## Resources

- [Flutter Documentation](https://flutter.dev/docs)
- [Dart Documentation](https://dart.dev/guides)
- [Flutter Cookbook](https://flutter.dev/docs/cookbook)
- [Pub.dev Packages](https://pub.dev)
- [Flutter Community](https://flutter.dev/community)

---

**Happy Coding! 🚀**
