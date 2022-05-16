import React from 'react';
import ApiManager from '../services/ApiManager.js';
import Box from "@mui/material/Box";
import { useNavigate } from "react-router-dom";
import ProductDataGrid from '../components/ProductDataGrid.js';
import { CircularProgress } from '@mui/material';


function MyProducts() {

    const [products, setProducts] = React.useState([]);
    const [data, setData] = React.useState([]);
    const [loaded, setLoaded] = React.useState(false);
    const [requestNeeded, setRequestNeeded] = React.useState(true);
    let navigate = useNavigate();

    const getProducts = async function () {
        setLoaded(false);
        const products = await ApiManager.getProductsByOwnerId();
        const groups = await ApiManager.getProductGroupsByOwnerId();
        const sales = await ApiManager.getSalesByUserId();
        let prodArr = [];
        for (let i = 0; i < products.length; i++) {
            if (products[i].isAvailable)
                products[i].isAvailable = "Available"
            else 
                products[i].isAvailable = "On sale"
            prodArr = prodArr.concat(products[i]);
            
        }
        let groupArr = groups.map((g) => {return {
            name: g.sampleProduct.name,
            description: g.sampleProduct.description,
            condition: g.sampleProduct.condition,
            isAvailable: g.sampleProduct.isAvailable ? "Available" : "On sale",
            unitPrice: g.sampleProduct.isAvailable ? null : sales.find(s => s.productGroup.id === g.id).unitPrice,
            currency: g.sampleProduct.isAvailable ? null : sales.find(s => s.productGroup.id === g.id).currency,
            id: "g_" + g.sampleProduct.groupID,
        }});
        setData(groupArr.concat(prodArr.filter(p => p.groupID === null)));
        setProducts(prodArr);
        setLoaded(true);
    }

    React.useEffect(() => {getProducts();}, [requestNeeded]);

    const refresh = () => {
        setRequestNeeded(!requestNeeded);
    }

    const deleteProduct = async function(productID) {
        //console.log("delete fv: ", productID);
        setLoaded(false);
        let response = await ApiManager.deleteProduct(productID);
        if (response) {
            setProducts(products.filter(p => p.id !== productID));
            setData(data.filter(p => p.id !== productID));
            setLoaded(true);
        }
        else {
            //TODO: NEM SIKERÜLT TÖRÖLNI :)
        }
    }

    const editProduct = function (productID) {
        navigate("/myproducts/" + productID);
    }

    const createNewGroup = async function (product) {
        setLoaded(false);
        product.isAvailable = product.isAvailable === "Available" ? true : false;
        await ApiManager.createNewProductGroupWithProduct(product)
        refresh();
    }

    const deleteGroup = async function(groupID) {
        setLoaded(false);
        let response = await ApiManager.deleteProductGroup(groupID.substring(2));
        if (response) {
            setData(data.filter(g => g.id !== groupID));
            setLoaded(true);
        }
        else {
            //TODO: NEM SIKERÜLT TÖRÖLNI :)
        }
    }

    const removeProductFromGroup = async function(groupID, idx) {
        setLoaded(false);
        let selectedProduct = products.filter(p => p.groupID === groupID.substring(2))[idx];
        selectedProduct.isAvailable = selectedProduct.isAvailable === "Available" ? true : false;
        selectedProduct.groupID = null;
        await ApiManager.editProduct(selectedProduct);
        selectedProduct.isAvailable = "Available";
        refresh();
    }

    const addProductToGroup = async function(productID, groupID) {
        setLoaded(false);
        let selectedProduct = products.filter(p => p.id === productID)[0];
        let selectedGroup = data.filter(g => g.id === groupID)[0];
        selectedProduct.isAvailable = selectedGroup.isAvailable === "Available" ? true : false;
        selectedProduct.groupID = groupID.substring(2);
        await ApiManager.editProduct(selectedProduct);
        selectedProduct.isAvailable = selectedProduct.isAvailable ? "Available" : "On sale";
        refresh();
    }

    const manageGroup = async function(groupID) {
        navigate("/myproducts/" + groupID);
    }

    const dataColumns = [
        {
            field: "name",
            headerName: "Name",
            description: "The name of the product",
            minWidth: 110,
            flex: 0
        }, {
            field: "description",
            headerName: "Description",
            description: "The description of the product",
            minWidth: 230,
            flex: 2
        }, {
            field: "condition",
            headerName: "Condition",
            description: "The condition of the product",
            minWidth: 110,
            flex: 1
        }, {
            field: "isAvailable",
            headerName: "Status",
            description: "This property shows whether the product is currently available, or it is on sale",
            minWidth: 110,
            flex: 0
        }, {
            field: "unitPrice",
            headerName: "Unit price",
            description: "Price of one product",
            minWidth: 110,
            flex: 0
        }, {
            field: "currency",
            headerName: "Currency",
            description: "The currency of the price",
            flex: 0
        }, {
            field: "id",
            headerName: "ID",
            flex: 1
        }];
        
    const groupColumn = [];

    if (loaded) {
        return(
            <ProductDataGrid 
                dataColumns = {dataColumns}
                groupColumns = {groupColumn}
                rows = {data}
                products = {products}
                deleteProduct = {deleteProduct.bind(this)}
                editProduct = {editProduct.bind(this)}
                newGroup = {createNewGroup.bind(this)}
                deleteGroup = {deleteGroup.bind(this)}
                removeProductFromGroup = {removeProductFromGroup.bind(this)}
                addProductToGroup = {addProductToGroup.bind(this)}
                manageGroup = {manageGroup.bind(this)}
            />
        );
    }

    return (
        <Box sx={{textAlign: "center", margin: "15% 0 18px 0"}}>
            <CircularProgress color="basic" size={60}/>
        </Box>
    );
}

export default MyProducts;