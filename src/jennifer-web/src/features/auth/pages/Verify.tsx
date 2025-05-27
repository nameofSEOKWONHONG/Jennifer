import { useState } from 'react';
import { TextField, Button, Container, Typography, Box } from '@mui/material';
import { useLocation, useNavigate } from 'react-router-dom';
import { verifySignUp } from '../api/auth';

export default function Verify() {
  const navigate = useNavigate();
  const location = useLocation();
  const userId = location.state?.userId;
  
  const [code, setCode] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleVerify = async () => {
    try {
      const response = await verifySignUp(userId, code);
      if (response?.isSuccess) {
        alert('Verification successful! You can now login.');
        navigate('/signin');
      } else {
        setError(response?.message || 'Verification failed');
      }
    } catch {
      setError('Verification error occurred');
    }
  };

  if (!userId) {
    return (
      <Box textAlign="center" mt={10}>
        <Typography variant="h6" color="error">Missing user ID</Typography>
        <Button variant="contained" onClick={() => navigate('/signup')}>Go Back to Sign Up</Button>
      </Box>
    );
  }

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
          <Typography variant="h4" align="center">Email Verification</Typography>
          <TextField
            label="Verification Code"
            value={code}
            onChange={(e) => setCode(e.target.value)}
            fullWidth
          />
          {error && <Typography color="error">{error}</Typography>}
          <Button variant="contained" onClick={handleVerify}>Verify</Button>
          <Button variant="outlined" onClick={() => navigate('/signin')}>
            Back to Sign In
          </Button>
        </Box>
      </Box>
    </Container>
  );
}
