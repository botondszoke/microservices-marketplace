import React from 'react';
import AppBar from '@mui/material/AppBar';
import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import {useNavigate} from 'react-router-dom';

function Controls() {
    //Hook
    const [anchorElUser, setAnchorElUser] = React.useState(null);

    const handleOpenUserMenu = (event) => {
      setAnchorElUser(event.currentTarget);
    };

    const handleCloseUserMenu = () => {
      setAnchorElUser(null);
    };

    const settings = ["Profile", "My products", "Add new product"];
    const links = ["/", "/myproducts", "/upload"]

    let navigate = useNavigate(); 
    const routeChange = (path) =>{ 
      navigate(path);
    }

    return (
      <React.Fragment>
        <AppBar position="fixed" color="basic">
            <Container maxWidth="xxl">
              <Toolbar disableGutters>
                <Typography variant="h4" className="navbarTitle">
                  <b><i>BuYTE</i></b>
                </Typography>

                <Box sx ={{ flexGrow: 1 }}>
                </Box>

                <Box sx={{ flexGrow: 0 }}>
                    <Tooltip title="Open settings">
                    <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                        <Avatar alt="Default avatar" src="images/avatar_default.jpg" />
                    </IconButton>
                    </Tooltip>
                    <Menu
                      sx={{ mt: '45px' }}
                      id="menu-appbar"
                      anchorEl={anchorElUser}
                      anchorOrigin={{
                          vertical: 'top',
                          horizontal: 'right',
                      }}
                      keepMounted
                      transformOrigin={{
                          vertical: 'top',
                          horizontal: 'right',
                      }}
                      open={Boolean(anchorElUser)}
                      onClose={handleCloseUserMenu}
                    >
                    {settings.map((setting) => (
                        <MenuItem key={setting} onClick={() => {routeChange(links[settings.indexOf(setting)]); handleCloseUserMenu(); }}>
                        <Typography textAlign="center">{setting}</Typography>
                        </MenuItem>
                    ))}
                    </Menu>
                </Box>
              </Toolbar>
            </Container>
        </AppBar>
        <Toolbar />
      </React.Fragment>
    );
  }

export default Controls;