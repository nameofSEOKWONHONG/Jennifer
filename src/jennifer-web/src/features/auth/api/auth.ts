export async function signIn(payload: { email: string; password: string }) {
  const response = await fetch('https://localhost:7288/api/v1/auth/signin', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    throw new Error('Failed to sign in');
  }

  return await response.json(); // returns ResultOfTokenResponse
}

export async function signUp(payload: {
  email: string;
  password: string;
  userName: string;
  phoneNumber: string;
}) {
  const response = await fetch('https://localhost:7288/api/v1/auth/signup', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  return await response.json();
}

export async function verifySignUp(userId: string, code: string) {
  const response = await fetch('https://localhost:7288/api/v1/auth/signup/verify', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, code }),
  });
  return await response.json();
}

export async function checkEmail(email: string) {    
    const response = await fetch('https://localhost:7288/api/v1/auth/check', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email }),
    });
    return await response.json();
};
