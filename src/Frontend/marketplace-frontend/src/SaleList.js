import React from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import ApiManager from './ApiManager';
import Box from '@mui/material/Box';
import SaleTile from './SaleTile';

class SaleList extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            sales: [],
            loaded: false,
        }
    }

    async componentDidMount() {
        const saleData = await ApiManager.getAllSales();
        for (let i = 0; i < saleData.length; i++) {
            this.AddSale(saleData[i]);
        }
        this.setState({
            loaded: true,
        });
    }

    AddSale(sale) {
        const saleList = this.state.sales.slice();
        this.setState({
            sales: saleList.concat([{
                id: sale.id,
                ownerID: sale.ownerID,
                productGroup: sale.productGroup,
                unitPrice: sale.unitPrice,
                currency: sale.currency,
            }]),
        });
    }

    render() {
        const sales = [];
        if (this.state.loaded) {
            for (let i = 0; i < this.state.sales.length; i++) {
                sales.push(
                    <SaleTile 
                        sale = {this.state.sales[i]}
                        key = {this.state.sales[i].id}
                    />
                );
            }
            const salesList = sales.length === 0 ? null : sales;
            return (
                <Box id="saleCatalog" sx={{display: "flex", flexWrap:"wrap", justifyContent:"space-between", ':after':{content: '""', flex: "auto"}}}>
                    {salesList}
                </Box>
            );
        };
        return (
            <CircularProgress />
        );
    };

}
export default SaleList;