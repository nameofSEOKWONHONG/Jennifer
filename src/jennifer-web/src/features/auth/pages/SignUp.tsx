import { useState } from 'react';
import { TextField, Button, Container, Typography, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { signUp } from '../api/auth';

export default function SignUp() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    email: '',
    password: '',
    userName: '',
    phoneNumber: '',
  });

  const [emailStatus, setEmailStatus] = useState<null | 'available' | 'taken'>(null);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (field: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm(prev => ({ ...prev, [field]: e.target.value }));
    if (field === 'email') setEmailStatus(null);
    setError(null);
  };

  const handleSignUp = async () => {
    try {
      const response = await signUp(form);
      if (response?.isSuccess) {
        alert('Signup successful. Please verify your email.');
        navigate('/signup/verify', {state: { userId: response.data } });
      } else {
        setError(response?.message || 'Signup failed');
      }
    } catch {
      setError('Signup error occurred');
    }
  };

    // 유효성 검사 함수들
    const isValidEmail = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    const isValidPassword = (password: string) => password.length >= 8;
    const isValidUserName = (name: string) => name.trim().length >= 2;
    const isValidPhoneNumber = (phone: string) => /^01[0|1|6-9]\d{7,8}$/.test(phone.replace(/-/g, ''));

    const isFormValid =
    isValidEmail(form.email) &&
    isValidPassword(form.password) &&
    isValidUserName(form.userName) &&
    isValidPhoneNumber(form.phoneNumber);


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
          <Typography variant="h4" align="center">Sign Up</Typography>
            <TextField
            label="Email"
            value={form.email}
            onChange={handleChange('email')}
            fullWidth
            error={!isValidEmail(form.email) || emailStatus === 'taken'}
            helperText={
                !isValidEmail(form.email)
                ? 'Please enter a valid email'
                : emailStatus === 'taken'
                    ? 'Email is already in use.'
                    : emailStatus === 'available'
                    ? 'Email is available.'
                    : ''
            }
            />
            <TextField
            label="Password"
            type="password"
            value={form.password}
            onChange={handleChange('password')}
            fullWidth
            error={form.password.length > 0 && !isValidPassword(form.password)}
            helperText={
                form.password.length > 0 && !isValidPassword(form.password)
                ? 'Password must be at least 6 characters'
                : ''
            }
            />

            <TextField
            label="User Name"
            value={form.userName}
            onChange={handleChange('userName')}
            fullWidth
            error={form.userName.length > 0 && !isValidUserName(form.userName)}
            helperText={
                form.userName.length > 0 && !isValidUserName(form.userName)
                ? 'User name must be at least 2 characters'
                : ''
            }
            />

            <TextField
            label="Phone Number"
            value={form.phoneNumber}
            onChange={handleChange('phoneNumber')}
            fullWidth
            error={form.phoneNumber.length > 0 && !isValidPhoneNumber(form.phoneNumber)}
            helperText={
                form.phoneNumber.length > 0 && !isValidPhoneNumber(form.phoneNumber)
                ? 'Invalid phone number format'
                : ''
            }
            />
          {error && <Typography color="error">{error}</Typography>}
            <Button
            variant="contained"
            onClick={handleSignUp}
            disabled={!isFormValid}
            >
            Register
            </Button>
          <Button variant="outlined" onClick={() => navigate('/signin')}>
            Back to Sign In
          </Button>
        </Box>
      </Box>
    </Container>
  );
}