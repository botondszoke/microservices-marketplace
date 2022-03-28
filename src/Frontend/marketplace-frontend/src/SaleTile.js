import React from 'react';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import {Link} from 'react-router-dom';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';

import ShoppingCartSharpIcon from '@mui/icons-material/ShoppingCartSharp';

class SaleTile extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            sale: props.sale,
        }
    }

    render() {
        return (
            <Card sx={{ maxWidth: "300px", minWidth:"256px", margin: "12px" }}>
                <CardMedia
                    component="img"
                    height="200"
                    image = {this.state.sale.productGroup.sampleProduct.pictureLinks.length === 0 ? '../images/default.jpg' : this.state.sale.productGroup.sampleProduct.pictureLinks[0]}
                    alt={this.state.sale.productGroup.sampleProduct.pictureLinks.length === 0 ? 'default picture' : this.state.sale.productGroup.sampleProduct.pictureLinks[0]}
                />
                <CardContent>
                    <Typography gutterBottom variant="h5" component="div">
                    {this.state.sale.productGroup.sampleProduct.name}
                    </Typography>
                    <Typography gutterBottom variant="subtitle1" component="div">
                    {this.state.sale.unitPrice + " " + this.state.sale.currency + "/pcs"}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                    Condition:
                    {this.state.sale.productGroup.sampleProduct.condition === null ? " Unknown" : " " + this.state.sale.productGroup.sampleProduct.condition}
                    </Typography>
                </CardContent>
                <CardActions id="productTileButtons" sx={{ justifyContent: "flex-end", padding: "8px 16px" }}>
                    <Tooltip title="Details and purchase">
                        <Link to={"/sale/" + this.state.sale.id}>
                            <Button size="small" variant="outlined" color="basic">
                                <ShoppingCartSharpIcon fontSize="medium" />
                            </Button>
                        </Link>
                    </Tooltip>
                </CardActions>
            </Card>
        )
    }
}
export default SaleTile;