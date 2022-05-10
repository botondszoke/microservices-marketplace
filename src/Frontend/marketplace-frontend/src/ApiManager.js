import React from 'react';
import Axios from 'axios';

const productApi = Axios.create({
    baseURL: 'http://localhost/api',
    withCredentials: true,
});

const saleApi = Axios.create({
    baseURL: 'http://localhost/api',
    withCredentials: true,
});

const blobBaseURL = "http://azure.localhost/devstoreaccount1/buyte-images-container/";

class ApiManager extends React.Component {

    static async getAllProducts() {
      const data = [];
      await productApi.get('/product/queries/all').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      data.forEach((e) => {
        e.pictureLinks.forEach((el) => {
          el = blobBaseURL + el;
        })
      })
      return data;
    }

    static async getProductsByOwnerId() {
      const data = [];
      await productApi.get('/product/queries/user/').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      })
      data.forEach((e) => {
        e.pictureLinks.forEach((el) => {
          el = blobBaseURL + el;
        })
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
      data.forEach((e) => {
        e.sampleProduct.pictureLinks.forEach((el) => {
          el = blobBaseURL + el;
        })
      })
      return data;
    }

    static async getProductGroup(id) {
      let productGroup;
      await productApi.get('/productgroup/queries/' + id).then((response) => {
        console.log(response);
        productGroup = response.data;
      })
      productGroup.sampleProduct.pictureLinks.forEach((e) => {
        e = blobBaseURL + e;
      })
      return productGroup;
    }

    static async getProductGroupsByOwnerId() {
      const data = [];
      await productApi.get('/productgroup/queries/user/').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++)
          data.push(response.data[i]);
      })
      data.forEach((e) => {
        e.sampleProduct.pictureLinks.forEach((el) => {
          el = blobBaseURL + el;
        })
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

    static async getSalesByUserId() {
      const productGroups = await this.getProductGroupsByOwnerId();
      const data = [];
      await saleApi.get('/sale/queries/user').then((response) => {
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
      let sellResponse;
      await productApi.put('/product/actions/sellByGroupId/' + productGroup.id, productGroup.sampleProduct).then((response) => {
        console.log(response);
        sellResponse = response;
      });
      return sellResponse.data;
    }

    static async uploadProduct(product) {
      let uploadResponse;
      await productApi.post('/product/actions/create/', product, {withCredentials: true}).then((response) => {
        console.log(response);
        uploadResponse = response;
      });
      return uploadResponse;
    }

    static async deleteProduct(productID) {
      let success = false;
      await productApi.delete('/product/actions/delete/' + productID).then((response) => {
        console.log(response);
        success = true;
      });
      return success;
    }
}
export default ApiManager;