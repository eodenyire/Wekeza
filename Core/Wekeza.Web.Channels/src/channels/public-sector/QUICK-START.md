# Public Sector Portal - Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Prerequisites
- Node.js 18+
- npm or yarn
- Git

### 1. Clone and Install

```bash
# Navigate to the project
cd Wekeza.Web.Channels

# Install dependencies
npm install
```

### 2. Configure Environment

Create `.env` file:

```env
VITE_API_URL=http://localhost:5000
```

### 3. Start Development Server

```bash
npm run dev
```

Visit: `http://localhost:5173/public-sector`

### 4. Login

Use test credentials (configure in backend):
- Username: `treasury.officer@wekeza.bank`
- Password: `Test@123`

## 📁 Project Structure

```
src/channels/public-sector/
├── components/     # Reusable UI components
├── pages/         # Page components
├── types/         # TypeScript definitions
├── utils/         # Utility functions
├── hooks/         # Custom React hooks
└── Layout.tsx     # Main layout
```

## 🎯 Key Features

### Securities Trading
- Navigate to `/public-sector/securities`
- Trade T-Bills, Bonds, and Stocks
- View portfolio

### Government Lending
- Navigate to `/public-sector/lending`
- Manage loan applications
- Approve/reject loans
- Disburse funds

### Banking Services
- Navigate to `/public-sector/banking`
- Manage accounts
- Process bulk payments
- Track revenues

### Grants & Philanthropy
- Navigate to `/public-sector/grants`
- View grant programs
- Submit applications

### Dashboard
- Navigate to `/public-sector/dashboard`
- View all metrics and charts

## 🔧 Common Tasks

### Add a New Page

1. Create page component:
```typescript
// pages/NewPage.tsx
export default function NewPage() {
  return <div>New Page</div>;
}
```

2. Add route:
```typescript
// PublicSectorPortal.tsx
<Route path="/new-page" element={<NewPage />} />
```

### Add a New Component

1. Create component:
```typescript
// components/MyComponent.tsx
interface MyComponentProps {
  title: string;
}

export default function MyComponent({ title }: MyComponentProps) {
  return <div>{title}</div>;
}
```

2. Export from index:
```typescript
// components/index.ts
export { default as MyComponent } from './MyComponent';
```

### Call an API

```typescript
import { handleApiCall } from '../utils/errorHandler';

const fetchData = async () => {
  try {
    const data = await handleApiCall<MyType>(
      () => fetch('/api/public-sector/endpoint')
    );
    // Use data
  } catch (error) {
    // Handle error
  }
};
```

### Show a Toast Notification

```typescript
import { useToast } from '../components/Toast';

function MyComponent() {
  const { success, error } = useToast();
  
  const handleAction = () => {
    success('Action completed successfully!');
    // or
    error('Something went wrong!');
  };
}
```

### Export Data

```typescript
import { exportToCSV } from '../utils/export';

const handleExport = () => {
  exportToCSV(data, 'filename');
};
```

## 🧪 Testing

### Run Tests
```bash
npm run test
```

### Run E2E Tests
```bash
npm run test:e2e
```

## 📦 Build for Production

```bash
npm run build
```

Output in `dist/` directory.

## 🐛 Troubleshooting

### Issue: API calls failing
**Solution**: Check `VITE_API_URL` in `.env`

### Issue: Authentication not working
**Solution**: Verify backend is running and JWT configuration

### Issue: Charts not displaying
**Solution**: Ensure Recharts is installed: `npm install recharts`

### Issue: TypeScript errors
**Solution**: Run `npm install` to ensure all types are installed

## 📚 Documentation

- **README.md** - Complete documentation
- **IMPLEMENTATION-COMPLETE.md** - Feature summary
- **DEPLOYMENT-GUIDE.md** - Deployment instructions

## 🔗 Useful Links

- [React Documentation](https://react.dev)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [Recharts](https://recharts.org/en-US/)
- [React Router](https://reactrouter.com/)

## 💡 Tips

1. **Use TypeScript** - All components are fully typed
2. **Reuse Components** - Check `components/` before creating new ones
3. **Follow Patterns** - Look at existing pages for patterns
4. **Error Handling** - Use `handleApiCall` for API calls
5. **Validation** - Use compliance utilities for validation
6. **Export** - Use export utilities for data export

## 🎨 Styling

The portal uses Tailwind CSS. Common patterns:

```typescript
// Card
<div className="bg-white rounded-lg shadow-md p-6">

// Button
<button className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">

// Input
<input className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500">

// Table
<table className="w-full">
  <thead className="bg-gray-50">
    <tr>
      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
```

## 🚦 Development Workflow

1. **Create Feature Branch**
```bash
git checkout -b feature/my-feature
```

2. **Make Changes**
- Write code
- Test locally
- Check TypeScript errors

3. **Commit Changes**
```bash
git add .
git commit -m "feat: add my feature"
```

4. **Push and Create PR**
```bash
git push origin feature/my-feature
```

## 📞 Get Help

- Check documentation files
- Review existing code for patterns
- Contact development team

---

**Happy Coding! 🎉**
