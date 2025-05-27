// src/components/layout/DashboardLayout.tsx
import { Box, Drawer, List, ListItemButton, ListItemText, AppBar, Toolbar, Typography } from '@mui/material';
import { Outlet, useNavigate } from 'react-router-dom';

const menuItems = [
  { text: 'Home', path: '/dashboard/home' },
  { text: 'Setting', path: '/dashboard/setting' },
  { text: 'Users', path: '/dashboard/users' },
];

export default function DashboardLayout() {
  const navigate = useNavigate();

  return (
    <Box display="flex">
      <Drawer variant="permanent" sx={{ width: 240, [`& .MuiDrawer-paper`]: { width: 240 } }}>
        <Toolbar />
        <List>
          {menuItems.map((item) => (
            <ListItemButton key={item.text} onClick={() => navigate(item.path)}>
              <ListItemText primary={item.text} />
            </ListItemButton>
          ))}
        </List>
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <AppBar position="fixed" sx={{ zIndex: 1201 }}>
          <Toolbar>
            <Typography variant="h6" noWrap component="div">
              Jennifer Dashboard
            </Typography>
          </Toolbar>
        </AppBar>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
}
