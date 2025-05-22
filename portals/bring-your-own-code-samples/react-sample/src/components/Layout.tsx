import React from 'react';
import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  AppBar,
  Box,
  CssBaseline,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
  Divider,
  Badge,
  Tooltip,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Dashboard as DashboardIcon,
  DirectionsCar as CarIcon,
  AttachMoney as SalesIcon,
  People as CustomersIcon,
  Notifications as NotificationsIcon,
  PersonAdd as PersonAddIcon,
} from '@mui/icons-material';
import ThemeToggle from './ThemeToggle';
import { AuthButton } from './AuthButton';

const drawerWidth = 240;

interface LayoutProps {
  children: React.ReactNode;
}

const menuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
  { text: 'Inventory', icon: <CarIcon />, path: '/inventory' },
  { text: 'Sales', icon: <SalesIcon />, path: '/sales' },
  { text: 'Customers', icon: <CustomersIcon />, path: '/customers' },
];

const isAuthenticated = (window as any)["Microsoft"]?.Dynamic365?.Portal?.User?.userRoles?.includes("Authenticated Users") ?? false;

if (isAuthenticated) {
  menuItems.push({ text: 'Sales Leads', icon: <PersonAddIcon />, path: '/leads' });
}

export default function Layout({ children }: LayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const drawer = (
    <div>
      <Toolbar sx={ { justifyContent: 'center' } }>
        <Box sx={ { display: 'flex', alignItems: 'center', gap: 1 } }>
          <CarIcon sx={ { color: 'primary.main', fontSize: 28 } } />
          <Typography variant="h6" sx={ { fontWeight: 600, color: 'primary.main' } }>
            CarSales
          </Typography>
        </Box>
      </Toolbar>
      <Divider />
      <List>
        { menuItems.map((item) => (
          <ListItem
            button
            key={ item.text }
            onClick={ () => navigate(item.path) }
            selected={ location.pathname === item.path }
            sx={ {
              borderRadius: '0 24px 24px 0',
              mr: 2,
              mb: 0.5,
              '&.Mui-selected': {
                backgroundColor: 'primary.main',
                color: 'white',
                '&:hover': {
                  backgroundColor: 'primary.dark',
                },
                '& .MuiListItemIcon-root': {
                  color: 'white',
                },
              },
              '&:hover': {
                backgroundColor: 'rgba(0, 0, 0, 0.04)',
              },
            } }
          >
            <ListItemIcon
              sx={ {
                minWidth: 40,
                color: location.pathname === item.path ? 'white' : 'primary.main',
              } }
            >
              { item.icon }
            </ListItemIcon>
            <ListItemText
              primary={ item.text }
              sx={ {
                '& .MuiTypography-root': {
                  fontWeight: location.pathname === item.path ? 600 : 400,
                  color: location.pathname === item.path ? 'white' : 'text.primary'
                }
              } }
            />
          </ListItem>
        )) }
      </List>
    </div>
  );

  return (
    <Box sx={ { display: 'flex' } }>
      <CssBaseline />        <AppBar
        position="fixed"
        sx={ {
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
          backgroundColor: 'background.paper',
          color: 'text.primary',
        } }
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={ handleDrawerToggle }
            sx={ { mr: 2, display: { sm: 'none' } } }
          >
            <MenuIcon />
          </IconButton>
          <Box sx={ { flexGrow: 1 } } />
          <Box sx={ { display: 'flex', alignItems: 'center', gap: 2 } }>
            <ThemeToggle />
            <Tooltip title="Notifications">
              <IconButton color="primary">
                <Badge badgeContent={ 3 } color="error">
                  <NotificationsIcon />
                </Badge>
              </IconButton>
            </Tooltip>
            <AuthButton />
          </Box>
        </Toolbar>
      </AppBar>
      <Box
        component="nav"
        sx={ { width: { sm: drawerWidth }, flexShrink: { sm: 0 } } }
      >
        <Drawer
          variant="temporary"
          open={ mobileOpen }
          onClose={ handleDrawerToggle }
          ModalProps={ {
            keepMounted: true,
          } }
          sx={ {
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': {
              boxSizing: 'border-box',
              width: drawerWidth,
              backgroundColor: 'background.paper',
            },
          } }
        >
          { drawer }
        </Drawer>
        <Drawer
          variant="permanent"
          sx={ {
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': {
              boxSizing: 'border-box',
              width: drawerWidth,
              backgroundColor: 'background.paper',
            },
          } }
          open
        >
          { drawer }
        </Drawer>
      </Box>
      <Box
        component="main"
        sx={ {
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          backgroundColor: 'background.default',
          minHeight: '100vh',
        } }
      >
        <Toolbar />
        { children }
      </Box>
    </Box>
  );
}
