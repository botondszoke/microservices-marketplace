import React from 'react';
import ApiManager from './ApiManager.js';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import FormControlLabel from '@mui/material/FormControlLabel';
import MenuItem from '@mui/material/MenuItem';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';



function Upload() {

    const [name, setName] = React.useState("");
    const [condition, setCondition] = React.useState("");
    const [description, setDescription] = React.useState("");
    const [ownerID, setOwnerID] = React.useState("");
    const [pictureNames, setPictureNames] = React.useState("");

    const [onSale, setOnSale] = React.useState(false);
    const [unitPrice, setUnitPrice] = React.useState("");
    const [currency, setCurrency] = React.useState("");

    const handleSubmit = () => {

    }

    const onSaleActions = [];
    onSaleActions.push(<TextField
        key={0}
        id="tfUnitPrice"
        label="Unit price"
        type="number"
        required={onSale}
        value={unitPrice}
        onChange={(event) => setUnitPrice(event.target.value)}
        color="basic"
        sx={{m: "0 8px"}}
    />);

    onSaleActions.push(
        <TextField
        select
        key={1}
        id="selCurrency"
        label="Currency"
        required={onSale}
        value={currency}
        onChange={(event) => setCurrency(event.target.value)}
        color="basic"
        sx={{m: "0 8px", width: "20ch"}}
    >
        <MenuItem value={"HUF"}>HUF</MenuItem>
        <MenuItem value={"EUR"}>EUR</MenuItem>
    </TextField>);
    
    return (
        <Container sx={{m: "auto"}}>
            <Box component="div" sx={{textAlign: "center", m: "36px"}}>
                <Typography variant="h3"><b>Upload your product</b></Typography>
                <Typography variant="h6" sx={{ m: "12px 0 12px 0" }}>On this page, you can upload a new product to the website. Don't forget to check the box at the end of the page, if you want to place it immediately on sale.</Typography>
            </Box>

            <Box
                component="form"
                sx = {{display: {xs: "block", md: "flex"}}}
                noValidate
                autoComplete="off"
            >
                <Box sx={{ '& .MuiTextField-root': { width: "calc(100% - 16px)", m: "8px" }, width: { xs: "100%", md: "50%"},}}>
                    <TextField
                        id="tfName"
                        label="Name"
                        required
                        value={name}
                        onChange={(event) => setName(event.target.value)}
                        color="basic"
                    />
                    <TextField
                        id="tfCondition"
                        label="Condition"
                        value={condition}
                        onChange={(event) => setCondition(event.target.value)}
                        color="basic"
                    />
                    <TextField
                        id="tfDescription"
                        label="Description"
                        multiline
                        minRows={3}
                        value={description}
                        onChange={(event) => setDescription(event.target.value)}
                        color="basic"
                    />
                    <TextField
                        id="tfOwnerID"
                        label="TEMP_FIELD_OwnerID"
                        variant="filled"
                        value={ownerID}
                        onChange={(event) => setOwnerID(event.target.value)}
                        color="basic"
                    />
                </Box>
                <Box sx={{ width: { xs: "calc(100%-16px)", md:"calc(50% - 16px)"},  padding: "8px"}}>
                    <TextField
                        id="tfPictureNames"
                        label="Pictures"
                        disabled
                        multiline
                        minRows={6}
                        maxRows={6}
                        fullWidth
                        value={pictureNames}
                        color="basic"
                    />
                    <Button
                        variant="contained"
                        component="label"
                        color="basic"
                        sx={{width:"100%", m: "12px 0 0 0"}}
                    >
                        Select pictures
                        <input
                            id="uploadImages"
                            type="file"
                            accept="image/*"
                            multiple
                            hidden
                            onChange={() => setPictureNames(Array.from(document.getElementById("uploadImages").files).map(f => f.name).join("\n"))}
                        />
                    </Button>
                </Box>
            </Box>
            <Divider sx={{m: "18px 0"}}/>
            <Box>
                <Box sx={{m:"18px 8px", display:"flex", justifyContent: "space-between"}}>
                    <FormControlLabel control={<Checkbox color="basic" checked={onSale} onChange={() => setOnSale(!onSale)} inputProps={{ 'aria-label': 'controlled' }} label="subid"/>} label = "I would like to place this product on sale now." />
                    <Button variant="contained"
                        color="basic"
                    >
                        Upload product
                    </Button>
                </Box>
                <Box sx={{m:"18px 0"}}>
                    {onSale ? onSaleActions : null}
                </Box>
            </Box>
        </Container>
    );
}

export default Upload;