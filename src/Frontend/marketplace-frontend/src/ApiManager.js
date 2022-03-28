import React from 'react';
import Axios from 'axios';

const productApi = Axios.create({
    baseURL: 'https://localhost:5001/api'
});

const saleApi = Axios.create({
    baseURL: 'https://localhost:6001/api'
});

class ApiManager extends React.Component {

    static async getAllProducts() {
      const data = [];
      await productApi.get('/Product').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getAllProductGroups() {
      const data = [];
      await productApi.get('/ProductGroup').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      return data;
    }

    static async getProductGroup(id) {
      let productGroup;
      await productApi.get('/ProductGroup/' + id).then((response) => {
        console.log(response);
        productGroup = response.data;
      })
      return productGroup;
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
}
export default ApiManager;