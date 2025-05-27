import { useEffect } from 'react';
import { Routes, Route, useNavigate, Navigate  } from 'react-router-dom';

import SignIn from './features/auth/pages/SignIn';
import SignUp from './features/auth/pages/SignUp';
import Verify from './features/auth/pages/Verify';

import Home from './features/dashboard/pages/Home';
import Setting from './features/dashboard/pages/Setting';
import User from './features/dashboard/pages/User';

import DashboardLayout from './components/layout/DashboardLayout';
import PrivateRoute from './components/guard/PrivateRoute';

import { useAuthStore } from './lib/useAuthStore';

export default function App() {
  const navigate = useNavigate();
  const { restoreFromStorage, isAuthenticated, loading } = useAuthStore();

  // 인증 상태 복원
  useEffect(() => {
    restoreFromStorage();
  }, []);

  // 루트 경로 진입 시 최초 진입 처리만 간단히
  useEffect(() => {
    if (!loading && window.location.pathname === '/') {
      navigate(isAuthenticated ? '/dashboard/home' : '/signin', { replace: true });
    }
  }, [isAuthenticated, loading]);

  return (
    <Routes>
      {/* 공개 라우트 */}
      <Route path="/signin" element={<SignIn />} />
      <Route path="/signup" element={<SignUp />} />
      <Route path="/signup/verify" element={<Verify />} />

      {/* 보호된 대시보드 라우트 */}
      <Route
        path="/dashboard"
        element={
          <PrivateRoute>
            <DashboardLayout />
          </PrivateRoute>
        }
      >
        <Route path="home" element={<Home />} />
        <Route path="setting" element={<Setting />} />
        <Route path="users" element={<User />} />
      </Route>

      {/* 예외 처리: 알 수 없는 경로 */}
      <Route path="*" element={<Navigate to="/signin" replace />} />
    </Routes>
  );
}