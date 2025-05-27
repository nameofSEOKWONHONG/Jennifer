import { useEffect } from 'react';
import { Routes, Route, useNavigate  } from 'react-router-dom';

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
  const { isAuthenticated, restoreFromStorage, loading } = useAuthStore();

  useEffect(() => {
    restoreFromStorage();
  }, []);

  useEffect(() => {
    if (!loading) {
      if (isAuthenticated) {
        navigate('/dashboard/home');
      } else {
        navigate('/signin');
      }
    }
  }, [isAuthenticated, loading]);

  if (loading) return <div>Loading...</div>;

  return (
    <Routes>
      {/* 공개 라우트 */}
      <Route path="/" element={<SignIn />} />
      <Route path="/signin" element={<SignIn />} />
      <Route path="/signup" element={<SignUp />} />
      <Route path="/signup/verify" element={<Verify />} />      

      {/* 보호된 대시보드 라우트 */}
      <Route path="/dashboard" element={
        <PrivateRoute>
          <DashboardLayout />
        </PrivateRoute>
      }>
        <Route path="home" element={<Home />} />
        <Route path="setting" element={<Setting />} />
        <Route path="users" element={<User />} />
      </Route>
    </Routes>
  );
}
