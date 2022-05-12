import React from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import ApiManager from './ApiManager';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import SaleTile from './SaleTile';
import Typography from '@mui/material/Typography';

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
        const emptyMessage = <Typography variant="h6" sx={{margin: "12px 0"}}>Currently, there are no sales yet. Be the first one, upload your products and put them on sale!</Typography>
        if (this.state.loaded) {
            for (let i = 0; i < this.state.sales.length; i++) {
                sales.push(
                    <SaleTile 
                        sale = {this.state.sales[i]}
                        key = {this.state.sales[i].id}
                    />
                );
            }
            const salesList = sales.length === 0 ? emptyMessage : sales;
            return (
                <Container>
                    <Box sx={{textAlign: "center", margin: "36px 0 18px 0"}}>
                        <Typography variant="h3"><b>Sales</b></Typography>
                        <Typography variant="h6" sx={{margin: "12px 0"}}>Check out our latest products, and buy them for a fix price!</Typography>
                    </Box>
                    <Divider />
                    <Box id="saleCatalog" sx={{display: "flex", flexWrap:"wrap", justifyContent:"space-between", margin: "18px 0"}}>
                        {salesList}
                    </Box>
                </Container>
            );
        };
        return (
            <CircularProgress />
        );
    };

}
export default SaleList;