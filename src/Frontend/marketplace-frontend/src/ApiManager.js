import React from 'react';
import Axios from 'axios';
import { Api } from '@mui/icons-material';

const productApi = Axios.create({
    baseURL: 'https://localhost/api'
});

const saleApi = Axios.create({
    baseURL: 'https://localhost/api'
});

class ApiManager extends React.Component {

    static async getAllProducts() {
      const data = [];
      await productApi.get('/Product/queries').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getProductsByOwnerId(ownerId) {
      const data = [];
      await productApi.get('/Product/queries/ownerId/' + ownerId).then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getAllProductGroups() {
      const data = [];
      await productApi.get('/ProductGroup/queries').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getProductGroup(id) {
      let productGroup;
      await productApi.get('/ProductGroup/queries/id/' + id).then((response) => {
        console.log(response);
        productGroup = response.data;
      })
      return productGroup;
    }

    static async getProductGroupsByOwnerId(ownerId) {
      const data = [];
      await productApi.get('/ProductGroup/queries/ownerId/' + ownerId).then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++)
          data.push(response.data[i]);
      })
      return data;
    }

    static async getAllSales() {
      const productGroups = await this.getAllProductGroups();
      const data = [];
      await saleApi.get('/Sale').then((response) => {
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
      await saleApi.get('/Sale/' + id).then((response) => {
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
      await productApi.put('/Product/actions/sellByGroupId/' + productGroup.id, productGroup.sampleProduct).then((response) => {
        console.log(response);
        sellResponse = response;
      });
      return sellResponse.data;
    }
}
export default ApiManager;