import React from "react";
import AddSharpIcon from '@mui/icons-material/AddSharp';
import ApiManager from "./ApiManager";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import Container from "@mui/material/Container";
import CircularProgress from "@mui/material/CircularProgress";
import Divider from "@mui/material/Divider";
import Paper from "@mui/material/Paper";
import RemoveSharpIcon from '@mui/icons-material/RemoveSharp';
import Carousel from "react-material-ui-carousel";
import Typography from "@mui/material/Typography";
import { TextField } from "@mui/material";


class SaleDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            id: props.id,
            sale: null,
            loaded: false,
            userQuantity: 1,
            userId: "",
        }
    }

    incrementQuantity() {
        if (this.state.userQuantity >= this.state.sale.productGroup.quantity)
            return;
        this.setState({
            userQuantity: this.state.userQuantity + 1,
        })
    }

    decrementQuantity() {
        if (this.state.userQuantity <= 1)
            return;
        this.setState({
            userQuantity: this.state.userQuantity - 1,
        })
    }

    async buyProduct() {      
        await ApiManager.sellProductFromGroup(this.state.sale.productGroup, this.state.userId);
    }

    async componentDidMount() {
        const item = await ApiManager.getSale(this.state.id);
        if (item.productGroup.sampleProduct.pictureLinks.length === 0)
            item.productGroup.sampleProduct.pictureLinks.push("../images/default.jpg");
        this.setState({
            sale: item,
            loaded: true,
        });
    }
    render() {
        if (this.state.loaded) {
            const actions = [];
            if (this.state.sale.productGroup.quantity <= 0) {
                actions.push(<Typography key={0} gutterBottom variant="h3" component="div"><b>{this.state.sale.productGroup.sampleProduct.name}</b></Typography>);
                actions.push(<Divider key={1} sx={{margin: "7px 0px 7px 0px"}}/>);
                actions.push(<Typography key={2} gutterBottom variant="h6" component="div" sx={{color: "red"}}>Unfortunately, this product is currently not available.</Typography>);
            }
            else {
                actions.push(<Typography key={0} gutterBottom variant="h3" component="div"><b>{this.state.sale.productGroup.sampleProduct.name}</b></Typography>);
                actions.push(<Divider key={1} sx={{margin: "7px 0px 7px 0px"}}/>);
                actions.push(<Typography key={2} gutterBottom variant="h6" component="div">Available: {this.state.sale.productGroup.quantity} on stock</Typography>);
                actions.push(<ButtonGroup key={3} color="basic" variant="contained">
                                <Button disabled={this.state.userQuantity <= 1} onClick={() => this.decrementQuantity()}><RemoveSharpIcon /></Button>
                                <Button disabled>{this.state.userQuantity}</Button>
                                <Button disabled={this.state.userQuantity >= this.state.sale.productGroup.quantity} onClick={() => this.incrementQuantity()}><AddSharpIcon /></Button>
                            </ButtonGroup>);
                actions.push(<div key={4} id="priceAndActions">
                                <Typography sx={{margin: "20px 20px 7px 0px"}} gutterBottom variant="h5" component="div">Total price: {this.state.sale.unitPrice * this.state.userQuantity} {this.state.sale.currency}</Typography>
                                <Button sx={{margin: "0px 7px 7px 0px", display:"inline-block", textTransform: "none", fontSize: "16px"}} color="basic" variant="contained" onClick={() => this.buyProduct()}>Buy it now!</Button>
                                <TextField sx={{margin: "0px 0px 7px 0px", display:"inline-block"}} label="TEMP_FIELD_Buyer_ID" color="basic" onChange={(event) => this.setState({
                                    userId: event.target.value,
                                })}>Buy it now!</TextField>
                            </div>);
            }
            return (
                <Paper elevation={3}>
                    <Container maxWidth="xxl" sx={{"& #pictureSmall div": {position:"static", height:"100% !important"}, "& #pictureBig div": {position:"static",  height:"100% !important"}, }}>

                    <Box id={this.state.sale.productGroup.sampleProduct.pictureLinks.length === 1 ? "pictureSmall" : "picturesSmall"} sx={{maxWidth: "100%", maxHeight: "100%", padding: "12px", display: {xs: "block", md: "none"}}}> 
                        <Carousel
                        animation="slide"
                        navButtonsAlwaysInvisible
                        >
                            {this.state.sale.productGroup.sampleProduct.pictureLinks.map(link => 
                                <img key ={link} src={link} className="sliderimg" alt="" width="100%"/>)
                            }
                        </Carousel>
                    </Box>

                    {/*navButtonsAlwaysInvisible and position static, height 100%!important 
                    are needed because of picture aspect ratio and Carousel inner render....*/}
                    <Box id="flexi" sx={{display:"flex", justifyContent:"space-evenly"}}>
                        <Box id={this.state.sale.productGroup.sampleProduct.pictureLinks.length === 1 ? "pictureBig" : "picturesBig"} sx={{flexGrow: 0, minWidth: "calc(45% - 24px)", maxWidth: "calc(45% - 24px)", padding: "12px", display: {xs: "none", md: "block"}, height:"100%"}}>
                            <Carousel
                            animation="slide"
                            navButtonsAlwaysInvisible
                            >
                                {this.state.sale.productGroup.sampleProduct.pictureLinks.map(link => 
                                    <img key={link} src={link} className="sliderimg" alt="" width="100%"/>)
                                }
                            </Carousel>
                        </Box>
                        
                        
                        <Box id="details" sx={{flexGrow: 1, minWidth: "calc(45% - 24px)", maxWidth: "calc(45% - 24px)", padding: "12px", margin:"auto 0", textAlign: {xs:"center", md:"left"}}}>
                            <div className="priceAndQuantity">
                                {actions}
                            </div>
                            <Divider sx={{margin: "7px 0px 7px 0px"}}/>
                            <div className="description">
                                <Typography gutterBottom variant="body1" component="div"><b>Condition:</b> {this.state.sale.productGroup.sampleProduct.condition}</Typography>
                                <Typography gutterBottom variant="body1" component="div"><b>Description:</b> {this.state.sale.productGroup.sampleProduct.description}</Typography>
                            </div>
                        </Box>
                    </Box>
                    <div className="reviews">
                    </div>
                    </Container>
                </Paper>
            );
        }
        return (<CircularProgress />);
    }
}

export default SaleDetails;