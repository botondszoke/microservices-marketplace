import React from 'react';
import Axios from 'axios';
import { Api } from '@mui/icons-material';

const productApi = Axios.create({
    baseURL: 'http://localhost/api'
});

const saleApi = Axios.create({
    baseURL: 'http://localhost/api'
});

class ApiManager extends React.Component {

    static async getAllProducts() {
      const data = [];
      await productApi.get('/product/queries/all').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getProductsByOwnerId(ownerId) {
      const data = [];
      await productApi.get('/product/queries/user/').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getAllProductGroups() {
      const data = [];
      await productApi.get('/productgroup/queries/all').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getProductGroup(id) {
      let productGroup;
      await productApi.get('/productgroup/queries/' + id).then((response) => {
        console.log(response);
        productGroup = response.data;
      })
      return productGroup;
    }

    static async getProductGroupsByOwnerId(ownerId) {
      const data = [];
      await productApi.get('/productgroup/queries/user/' + ownerId).then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++)
          data.push(response.data[i]);
      })
      return data;
    }

    static async getAllSales() {
      const productGroups = await this.getAllProductGroups();
      const data = [];
      await saleApi.get('/sale/queries/all').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      const newData = data.map(sale => {
        return {
          id: sale.id,
          ownerID: sale.ownerID,
          productGroup: productGroups.find(pg => pg.id === sale.productGroupID),
          unitPrice: sale.unitPrice,
          currency: sale.currency,
        }
      })
      return newData;
    }

    static async getSale(id) {
      let sale;
      await saleApi.get('/sale/queries' + id).then((response) => {
        console.log(response);
        sale = response.data;
      })
      return {
        id: sale.id,
        ownerID: sale.ownerID,
        productGroup: await this.getProductGroup(sale.productGroupID),
        unitPrice: sale.unitPrice,
        currency: sale.currency,
      }
    }

    static async sellProductFromGroup(productGroup, newOwnerId) {
      productGroup.sampleProduct.ownerID = newOwnerId;
      console.log(productGroup);
      var sellResponse;
      await productApi.put('/product/actions/sellByGroupId/' + productGroup.id, productGroup.sampleProduct).then((response) => {
        console.log(response);
        sellResponse = response;
      });
      return sellResponse.data;
    }
}
export default ApiManager;