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
import { useKeycloak } from '@react-keycloak/web';



function Upload() {

    const [name, setName] = React.useState("");
    const [condition, setCondition] = React.useState("");
    const [description, setDescription] = React.useState("");
    const [ownerID, setOwnerID] = React.useState("");
    const [pictureNames, setPictureNames] = React.useState("");

    const [onSale, setOnSale] = React.useState(false);
    const [unitPrice, setUnitPrice] = React.useState("");
    const [currency, setCurrency] = React.useState("");

    const {keycloak, initialized} = useKeycloak();

    const encodePicturesAndSend = (from, idx, obj, reader) => {
        reader.onload = function () {
            let result = reader.result.split(",");
            result.shift();
            obj.encodedPictures = obj.encodedPictures.concat(result.join(""));
            if (idx !== obj.pictureLinks.length-1) {
                encodePicturesAndSend(from, idx+1, obj, reader)
            }
            else {
                ApiManager.uploadProduct(obj);
            }
        }
        reader.onerror = function (error) {
            console.log('Error: ', error);
        };
        if (idx >= from.length) {
            ApiManager.uploadProduct(obj);
        }
        else {
            reader.readAsDataURL(from[idx]);
        }
    }

    const handleSubmit = () => {        
        let files = Array.from(document.getElementById("uploadImages").files);
        let product = {
            id: "",
            ownerID: initialized ? keycloak.tokenParsed.email : "",
            groupID: null,
            name: name,
            description: description === "" ? null : description,
            condition: condition === "" ? null : condition,
            isAvailable: true,
            pictureLinks: pictureNames.split('\n'),
            encodedPictures: [],
        };

        encodePicturesAndSend(files, 0, product, new FileReader());
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
    //TODO: WARNING: CSAK AZ ELSŐ 5 KÉP LESZ FIGYELEMBE VÉVE!
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
                            onChange={() => {
                                let pictureNames = Array.from(document.getElementById("uploadImages").files).map(f => f.name);
                                if (pictureNames.length <= 5) {
                                    setPictureNames(pictureNames.join("\n"));
                                }
                                else {
                                    let firstNames = [];
                                    for (let i = 0; i < 5; i++) {
                                        firstNames = firstNames.concat(pictureNames[i]);
                                    }
                                    setPictureNames(firstNames.join("\n"));
                                }
                            }}
                        />
                    </Button>
                </Box>
            </Box>
            <Divider sx={{m: "18px 0"}}/>
            <Box>
                <Box sx={{m:"18px 8px", display:"flex", justifyContent: "space-between"}}>
                    <FormControlLabel control={<Checkbox color="basic" checked={onSale} onChange={() => setOnSale(!onSale)} inputProps={{ 'aria-label': 'controlled' }} label="subid"/>} label = "I would like to place this product on sale now." />
                    <Button variant="contained" color="basic" onClick={handleSubmit}>
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