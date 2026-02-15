import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from '@/contexts/AuthContext';

// Channel imports
import PublicWebsite from '@/channels/public/PublicWebsite';
import PersonalBanking from '@/channels/personal/PersonalBanking';
import CorporateBanking from '@/channels/corporate/CorporateBanking';
import SMEBanking from '@/channels/sme/SMEBanking';
import PublicSectorPortal from '@/channels/public-sector/PublicSectorPortal';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Public Website */}
          <Route path="/*" element={<PublicWebsite />} />
          
          {/* Personal Banking */}
          <Route path="/personal/*" element={<PersonalBanking />} />
          
          {/* Corporate Banking */}
          <Route path="/corporate/*" element={<CorporateBanking />} />
          
          {/* SME Banking */}
          <Route path="/sme/*" element={<SMEBanking />} />
          
          {/* Public Sector Portal */}
          <Route path="/public-sector/*" element={<PublicSectorPortal />} />
          
          {/* Fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
