import { useState } from 'react';
import { TextField, Button, Container, Typography, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { signIn } from '../api/auth';
import { useAuthStore } from '../../../lib/useAuthStore';

export default function SignIn() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [emailStatus, setEmailStatus] = useState<null | 'available' | 'taken'>(null);
  const [error, setError] = useState<string | null>(null);
  const { setTokens } = useAuthStore();

  const isValidEmail = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const isValidPassword = (password: string) => password.length >= 8;

  const handleChange = (field: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm(prev => ({ ...prev, [field]: e.target.value }));
    if (field === 'email') setEmailStatus(null);
    setError(null);
  };

  const handleLogin = async () => {
    try {
      const response = await signIn(form);
      if (response?.isSuccess) {
        setTokens(response.data.accessToken, response.data.refreshToken);
        navigate('/dashboard');
      } else {
        setError(response?.message || 'Login failed');
      }
    } catch {
      setError('Login error occurred');
    }
  };

   return (
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: '100vh',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <Box width="100%" display="flex" flexDirection="column" gap={2}>
          <Typography variant="h4" align="center">Login</Typography>
            <TextField
            label="Email"
            type="email"
            value={form.email}
            onChange={handleChange('email')}
            fullWidth
            error={!isValidEmail(form.email) && form.email.length > 0}
            helperText={
                !isValidEmail(form.email) && form.email.length > 0
                ? 'Please enter a valid email address'
                : emailStatus === 'available'
                    ? 'Email is not registered. You can sign up.'
                    : emailStatus === 'taken'
                    ? 'Email is registered. You can login.'
                    : ''
            }
            />
            <TextField
            label="Password"
            type="password"
            value={form.password}
            onChange={handleChange('password')}
            fullWidth
            error={!isValidPassword(form.password) && form.password.length > 0}
            helperText={
                !isValidPassword(form.password) && form.password.length > 0
                ? 'Password must be at least 8 characters'
                : ''
            }
            />
          {error && <Typography color="error">{error}</Typography>}
            <Button
            variant="contained"
            onClick={handleLogin}
            disabled={
                !isValidEmail(form.email) ||
                !isValidPassword(form.password) ||
                emailStatus === 'available'
            }
            >
            Login
            </Button>
          <Button variant="outlined" onClick={() => navigate('/signup')}>
            Go to Sign Up
          </Button>
        </Box>
      </Box>
    </Container>
  );
}