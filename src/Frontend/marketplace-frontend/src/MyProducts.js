import React from 'react';
import ApiManager from './ApiManager.js';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import ProductDataGrid from './ProductDataGrid.js';

class MyProducts extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            products: [],
            data: [],
            loaded: false,
        }
    }

    async getProducts() {
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
        this.setState({
            data: groups.map((g) => {return {
                name: g.sampleProduct.name,
                description: g.sampleProduct.description,
                condition: g.sampleProduct.condition,
                isAvailable: g.sampleProduct.isAvailable ? "Available" : "On sale",
                unitPrice: sales.find(s => s.productGroup.id === g.id).unitPrice,
                currency: sales.find(s => s.productGroup.id === g.id).currency,
                id: "g_" + g.sampleProduct.groupID,
            }}),
            products: prodArr,
        })
        this.setState({
            data: this.state.data.concat(products.filter(p => p.groupID === null)),
        })
        this.setState({
            loaded: true,
        });
    }

    async componentDidMount() {
        this.getProducts();
    }

    async deleteProduct(productID) {
        //console.log("delete fv: ", productID);
        let response = await ApiManager.deleteProduct(productID);
        if (response) {
            this.setState({
                products: this.state.products.filter(p => p.id !== productID),
                data: this.state.data.filter(p => p.id !== productID)
            });
        }
        else {
            //TODO: NEM SIKERÜLT TÖRÖLNI :)
        }
    }

    render() {
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

        return(
            <ProductDataGrid 
                dataColumns = {dataColumns}
                groupColumns = {groupColumn}
                rows = {this.state.data}
                products = {this.state.products}
                deleteProduct = {this.deleteProduct.bind(this)}
            />
        );
    }
}

export default MyProducts;