import { useState } from 'react';
import { CheckCircle } from 'lucide-react';

export default function OpenAccountPage() {
  const [step, setStep] = useState(1);

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 className="text-4xl font-bold mb-8 text-center">Open Your Account</h1>
      
      {/* Progress Steps */}
      <div className="flex justify-center mb-12">
        <div className="flex items-center space-x-4">
          <div className={`flex items-center ${step >= 1 ? 'text-wekeza-blue' : 'text-gray-400'}`}>
            <div className={`w-10 h-10 rounded-full flex items-center justify-center ${step >= 1 ? 'bg-wekeza-blue text-white' : 'bg-gray-200'}`}>
              {step > 1 ? <CheckCircle className="h-6 w-6" /> : '1'}
            </div>
            <span className="ml-2 font-medium">Basic Info</span>
          </div>
          <div className="w-16 h-1 bg-gray-300"></div>
          <div className={`flex items-center ${step >= 2 ? 'text-wekeza-blue' : 'text-gray-400'}`}>
            <div className={`w-10 h-10 rounded-full flex items-center justify-center ${step >= 2 ? 'bg-wekeza-blue text-white' : 'bg-gray-200'}`}>
              {step > 2 ? <CheckCircle className="h-6 w-6" /> : '2'}
            </div>
            <span className="ml-2 font-medium">Documents</span>
          </div>
          <div className="w-16 h-1 bg-gray-300"></div>
          <div className={`flex items-center ${step >= 3 ? 'text-wekeza-blue' : 'text-gray-400'}`}>
            <div className={`w-10 h-10 rounded-full flex items-center justify-center ${step >= 3 ? 'bg-wekeza-blue text-white' : 'bg-gray-200'}`}>
              3
            </div>
            <span className="ml-2 font-medium">Complete</span>
          </div>
        </div>
      </div>

      {/* Step Content */}
      <div className="card">
        {step === 1 && (
          <div>
            <h2 className="text-2xl font-bold mb-6">Step 1: Basic Information</h2>
            <form className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <input type="text" placeholder="First Name" className="input" required />
                <input type="text" placeholder="Last Name" className="input" required />
              </div>
              <input type="email" placeholder="Email Address" className="input" required />
              <input type="tel" placeholder="Phone Number" className="input" required />
              <input type="date" placeholder="Date of Birth" className="input" required />
              <input type="text" placeholder="ID Number" className="input" required />
              <button type="button" onClick={() => setStep(2)} className="btn btn-primary w-full">
                Continue
              </button>
            </form>
          </div>
        )}

        {step === 2 && (
          <div>
            <h2 className="text-2xl font-bold mb-6">Step 2: Upload Documents</h2>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-2">ID Document</label>
                <input type="file" className="input" accept="image/*,application/pdf" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Proof of Address</label>
                <input type="file" className="input" accept="image/*,application/pdf" />
              </div>
              <div className="flex gap-4">
                <button type="button" onClick={() => setStep(1)} className="btn btn-secondary flex-1">
                  Back
                </button>
                <button type="button" onClick={() => setStep(3)} className="btn btn-primary flex-1">
                  Submit
                </button>
              </div>
            </div>
          </div>
        )}

        {step === 3 && (
          <div className="text-center py-8">
            <CheckCircle className="h-16 w-16 text-wekeza-green mx-auto mb-4" />
            <h2 className="text-2xl font-bold mb-4">Application Submitted!</h2>
            <p className="text-gray-600 mb-6">
              Your account application has been submitted successfully. We'll review your documents and
              get back to you within 24 hours.
            </p>
            <p className="text-sm text-gray-500">
              Application ID: <span className="font-mono font-semibold">WKZ-2026-{Math.random().toString(36).substr(2, 9).toUpperCase()}</span>
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
