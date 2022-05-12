import React from 'react';
import AppBar from '@mui/material/AppBar';
import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { useKeycloak } from '@react-keycloak/web';
import { useNavigate } from 'react-router-dom';

function Controls() {
    const [anchorElUser, setAnchorElUser] = React.useState(null);
    const { keycloak, initialized } = useKeycloak();

    const handleOpenUserMenu = (event) => {
      setAnchorElUser(event.currentTarget);
    };

    const handleCloseUserMenu = () => {
      setAnchorElUser(null);
    };

    const settings = ["Profile", "My products", "Upload new product", "Log out"];
    const links = ["http://keycloak.localhost/auth/realms/buyte/account/#/", "/myproducts", "/upload"]

    let navigate = useNavigate(); 
    const routeChange = (path) =>{ 
      if(path.startsWith("http://"))
        window.location = path;
      else
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
                <Box sx={{flexGrow: 0, m: "0 30px"}}>
                  <Button sx={{textTransform: "none", fontSize: "16px"}} variant="text" color="white" className="salesLink" onClick={() => {routeChange("/sales")}}>
                    Sales
                  </Button>
                </Box>
                <Box sx ={{ flexGrow: 1 }}>
                </Box>

                <Box sx={{ flexGrow: 0 }}>
                    {!keycloak.authenticated && initialized && (
                      <Button sx={{margin: "0px 7px 7px 0px", display:"inline-block", textTransform: "none", fontSize: "16px"}} color="white" variant="text" onClick={() => keycloak.login()}>Login</Button>
                    )}
                    {keycloak.authenticated && initialized && (
                      <Tooltip title="Open settings">
                        <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                            <Avatar alt= {keycloak.tokenParsed.preferred_username} src="images/avatar_default.jpg" />
                        </IconButton>
                      </Tooltip>
                    )}
                    {keycloak.authenticated && initialized && (
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
                    {settings.map((option) => (
                        <MenuItem key={option} onClick={() => {option !== "Log out" ? routeChange(links[settings.indexOf(option)]) : keycloak.logout();  handleCloseUserMenu(); }}>
                        <Typography textAlign="center">{option}</Typography>
                        </MenuItem>
                    ))}
                    </Menu>
                    )}
                </Box>
              </Toolbar>
            </Container>
        </AppBar>
        <Toolbar />
      </React.Fragment>
    );
  }

export default Controls;