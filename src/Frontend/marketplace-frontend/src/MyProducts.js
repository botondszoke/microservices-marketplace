import React from 'react';
import ApiManager from './ApiManager.js';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import ProductDataGrid from './ProductDataGrid.js';

class MyProducts extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            ownerID: "",
            products: [],
            data: [],
            loaded: false,
        }
    }

    async getProducts() {
        const products = await ApiManager.getProductsByOwnerId(this.state.ownerID);
        const groups = await ApiManager.getProductGroupsByOwnerId(this.state.ownerID);
        for (let i = 0; i < products.length; i++) {
            if (products[i].isAvailable)
                products[i].isAvailable = "Available"
            else
                products[i].isAvailable = "On sale"
            this.setState({
                products: this.state.products.concat(products[i])});
        }
        this.setState({
            data: groups.map((g) => {return {
                name: g.sampleProduct.name,
                description: g.sampleProduct.description,
                condition: g.sampleProduct.condition,
                isAvailable: g.sampleProduct.isAvailable ? "Available" : "On sale",
                id: "g_" + g.sampleProduct.groupID,
            }})
        })
        this.setState({
            data: this.state.data.concat(products.filter(p => p.groupID === null)),
        })
        this.setState({
            loaded: true,
        });
    }

    async componentDidMount() {
        //Later getProducts....
    }

    render() {
        console.log(this.state.data);
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
                headerName: "State",
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

        if(!this.state.loaded)
            return (<div>
                <TextField
                        id="tfOwnerId"
                        label="OwnerID"
                        required
                        value={this.state.ownerID}
                        onChange={(event) => this.setState({ownerID: event.target.value})}
                        color="basic"
                />
                <Button color="basic" variant="contained" onClick={() => this.getProducts()}>Set</Button>
                </div>
            );

        return(
            <ProductDataGrid 
                dataColumns = {dataColumns}
                groupColumns = {groupColumn}
                rows = {this.state.data}
                products = {this.state.products}
            />
        );
    }
}

export default MyProducts;