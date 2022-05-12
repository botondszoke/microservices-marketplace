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
import { useNavigate } from "react-router-dom";
import { CircularProgress } from '@mui/material';



function Details(props) {

    //let product = {};

    const [product, setProduct] = React.useState({});
    const [group, setGroup] = React.useState({});
    const [name, setName] = React.useState("");
    const [condition, setCondition] = React.useState("");
    const [description, setDescription] = React.useState("");
    const [pictureNames, setPictureNames] = React.useState("");

    const [saleID, setSaleID] = React.useState("");
    const [onSale, setOnSale] = React.useState(false);
    const [unitPrice, setUnitPrice] = React.useState("");
    const [currency, setCurrency] = React.useState("");

    const {keycloak, initialized} = useKeycloak();
    let navigate = useNavigate();
    const [loaded, setLoaded] = React.useState(props.mode === "edit" ? false : true);

    const getData = async function () {
        if (props.mode === "edit") {
            const respProd = await ApiManager.getProduct(props.id);
            setProduct(respProd);
            setName(respProd.name);
            setCondition(respProd.condition === null ? "" : respProd.condition);
            setDescription(respProd.description === null ? "" : respProd.description);
            setPictureNames(respProd.pictureLinks.join("\n"));
            setLoaded(true);
        }
        else if (props.mode === "editGroup") {
            const respGroup = await ApiManager.getProductGroup(props.id.substring(2));
            setGroup(respGroup);
            setProduct(respGroup.sampleProduct);
            setName(respGroup.sampleProduct.name);
            setCondition(respGroup.sampleProduct.condition === null ? "" : respGroup.sampleProduct.condition);
            setDescription(respGroup.sampleProduct.description === null ? "" : respGroup.sampleProduct.description);
            setPictureNames(respGroup.sampleProduct.pictureLinks.join("\n"));
            setOnSale(!respGroup.sampleProduct.isAvailable);
            if (!respGroup.sampleProduct.isAvailable) {
                const respSale = await ApiManager.getSaleByProductGroupId(props.id.substring(2));
                setUnitPrice(respSale.unitPrice);
                setCurrency(respSale.currency);
                setSaleID(respSale.id);
            }
            setLoaded(true);
        }
    }

    React.useEffect(() => {getData();}, []);

    const uploadAndToProduct = async (finalProduct) => {
        await ApiManager.uploadProduct(finalProduct);
        navigate("/myproducts");
    }

    const editAndToProduct = async (finalProduct) => {
        await ApiManager.editProduct(finalProduct);
        navigate("/myproducts");
    }

    const encodePictures = async (from, idx, obj, reader, func) => {
        reader.onload = async function () {
            let result = reader.result.split(",");
            result.shift();
            obj.encodedPictures = obj.encodedPictures.concat(result.join(""));
            if (idx !== obj.pictureLinks.length-1) {
                await encodePictures(from, idx+1, obj, reader, func);
                //return res;
            }
            else {
                await func(obj);
                //return obj;
            }
        }
        reader.onerror = function (error) {
            console.log('Error: ', error);
        };
        if (idx >= from.length) {
            await func(obj);
        }
        else {
            reader.readAsDataURL(from[idx]);
        }
    }

    const handleSubmitUpload = async () => {        
        let files = Array.from(document.getElementById("uploadImages").files);
        let finalProduct = {
            id: "",
            ownerID: initialized ? keycloak.tokenParsed.email : "",
            groupID: null,
            name: name,
            description: description === "" ? null : description,
            condition: condition === "" ? null : condition,
            isAvailable: true,
            pictureLinks: pictureNames === "" ? [] : pictureNames.split('\n'),
            encodedPictures: [],
        };

        await encodePictures(files, 0, finalProduct, new FileReader(), uploadAndToProduct);
    }

    const handleSubmitEdit = async () => {
        let files = Array.from(document.getElementById("uploadImages").files);
        let finalProduct = {
            id: product.id,
            ownerID: product.ownerID,
            groupID: product.groupID,
            name: name,
            description: description === "" ? null : description,
            condition: condition === "" ? null : condition,
            isAvailable: product.isAvailable,
            pictureLinks: pictureNames === "" ? [] : pictureNames.split('\n'),
            encodedPictures: [],
        };

        if (files.length !== 0) {
            await encodePictures(files, 0, finalProduct, new FileReader(), editAndToProduct);
        }
        else {
            editAndToProduct(finalProduct);
        }
    }

    const handleSubmitGroupEdit = async () => {
        if (saleID === "" && onSale) {
            let finalSale = {
                id : "",
                ownerID: initialized ? keycloak.tokenParsed.email : "",
                productGroupID: group.id,
                unitPrice: parseFloat(unitPrice),
                currency: currency,
            }
            let response = await ApiManager.createSale(finalSale);
            if (response) {
                let finalGroup = group;
                finalGroup.sampleProduct.isAvailable = false;
                let response = await ApiManager.editProductGroup(finalGroup);
            }
        }
        else if (saleID !== "" && onSale){
            let finalSale = {
                id : saleID,
                ownerID: initialized ? keycloak.tokenParsed.email : "",
                productGroupID: group.id,
                unitPrice: parseFloat(unitPrice),
                currency: currency,
            }
            let response = await ApiManager.editSale(finalSale);
        }
        else if (saleID !== "" && !onSale) {
            let response = await ApiManager.deleteSale(saleID);
            if (response) {
                let finalGroup = group;
                finalGroup.sampleProduct.isAvailable = true;
                let response = await ApiManager.editProductGroup(finalGroup);
            }
        }
        navigate("/myproducts");
    }

    const pageInfo = [];
    if (props.mode === "upload") {
        pageInfo.push(<Typography key={0} variant="h3"><b>Upload your product</b></Typography>);
        pageInfo.push(<Typography key={1} variant="h6" sx={{ m: "12px 0 12px 0" }}>On this page, you can upload a new product to the website.</Typography>);
    }
    else if (props.mode === "edit"){
        pageInfo.push(<Typography key={0} variant="h3"><b>Edit your product</b></Typography>);
        pageInfo.push(<Typography key={1} variant="h6" sx={{ m: "12px 0 12px 0" }}>You are currently editing the following product: {product.name} - {product.id} </Typography>);
    }
    else if (props.mode === "editGroup"){
        pageInfo.push(<Typography key={0} variant="h3"><b>Manage your group</b></Typography>);
        pageInfo.push(<Typography key={1} variant="h6" sx={{ m: "12px 0 12px 0" }}>You are currently managing the following group: {product.name} - {"g_" + product.groupID} </Typography>);
    }

    const submitButton = [];
    if (props.mode === "upload") {
        submitButton.push(<Button key={0} variant="contained" color="basic" onClick={handleSubmitUpload}>Upload product</Button>);
    }
    else if (props.mode === "edit") {
        submitButton.push(<Button key={0} variant="contained" color="basic" onClick={handleSubmitEdit}>Save changes</Button>)
    }
    else if (props.mode === "editGroup") {
        submitButton.push(<Button key={0} variant="contained" color="basic" onClick={handleSubmitGroupEdit}>Save changes</Button>)
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
        size="small"
        sx={{m: "0 8px"}}
        />
    );

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
        size="small"
        sx={{m: "0 0 0 8px", width: "20ch"}}
        >
            <MenuItem value={"HUF"}>HUF</MenuItem>
            <MenuItem value={"EUR"}>EUR</MenuItem>
        </TextField>
    );

    const groupSettings = [];

    groupSettings.push(
        <Box key={0} sx={{m:"18px 8px", display:"flex", justifyContent: "space-between"}}>
            <FormControlLabel control={<Checkbox color="basic" checked={onSale} onChange={() => setOnSale(!onSale)} inputProps={{ 'aria-label': 'controlled' }} label="subid"/>} label = "On sale" />
            <Box sx={{m:"0", p:"0"}}>
                {onSale ? onSaleActions : null}
            </Box>
        </Box>
    );

    groupSettings.push(
        <Divider key={1} sx={{m: "18px 0"}}/>
    );

    if (!loaded || (props.mode === "edit" && (typeof name === "undefined" || typeof condition === "undefined" || typeof description === "undefined" || typeof pictureNames === "undefined" ))) {
        return (
            <CircularProgress />
        );
    }
    return (
        <Container sx={{m: "auto"}}>
            <Box component="div" sx={{textAlign: "center", m: "36px"}}>
                {pageInfo}
            </Box>

            <Box
                component="form"
                sx = {{display: {xs: "block", md: "flex"}}}
                Validate
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
                        disabled={props.mode === "editGroup"}
                    />
                    <TextField
                        id="tfCondition"
                        label="Condition"
                        value={condition}
                        onChange={(event) => setCondition(event.target.value)}
                        color="basic"
                        disabled={props.mode === "editGroup"}
                    />
                    <TextField
                        id="tfDescription"
                        label="Description"
                        multiline
                        minRows={3}
                        value={description}
                        onChange={(event) => setDescription(event.target.value)}
                        color="basic"
                        disabled={props.mode === "editGroup"}
                    />
                </Box>
                <Box sx={{ width: { xs: "calc(100%-16px)", md:"calc(50% - 16px)"},  padding: "8px"}}>
                    
                    <TextField
                        id="tfPictureNames"
                        label="Pictures (max. 5)"
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
                        disabled={props.mode === "editGroup"}
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
                {props.mode === "editGroup" ? groupSettings : null}
                <Box sx={{m:"18px 8px", display:"flex", justifyContent: "right"}}>
                    {submitButton}
                </Box>
            </Box>
        </Container>
    );
}

export default Details;