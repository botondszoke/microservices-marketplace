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

const blobBaseURL = "http://localhost/devstoreaccount1/buyte-images-container/";

class ApiManager extends React.Component {

    static getBlobBaseURL() {
      return blobBaseURL;
    }

    static async getAllProducts() {
      const data = [];
      await productApi.get('/product?filter=all').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      });
      return data;
    }

    static async getProductsByOwnerId() {
      const data = [];
      await productApi.get('/product?filter=user').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      });
      return data;
    }

    static async getAllProductGroups() {
      const data = [];
      await productApi.get('/productgroup?filter=all').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++) {
          data.push(response.data[i]);
        }
      });
      return data;
    }

    static async getProductGroup(id) {
      let productGroup;
      await productApi.get('/productgroup/' + id).then((response) => {
        console.log(response);
        productGroup = response.data;
      });
      return productGroup;
    }

    static async getProductGroupsByOwnerId() {
      const data = [];
      await productApi.get('/productgroup?filter=user').then((response) => {
        console.log(response);
        for (let i = 0; i < response.data.length; i++)
          data.push(response.data[i]);
      });
      return data;
    }

    static async getAllSales() {
      const productGroups = await this.getAllProductGroups();
      const data = [];
      await saleApi.get('/sale?filter=all').then((response) => {
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
      await saleApi.get('/sale?filter=user').then((response) => {
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
      await saleApi.get('/sale/' + id).then((response) => {
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

    static async getSaleByProductGroupId(id) {
      let sale = {};
      await saleApi.get('/sale?filter=productGroupId&productGroupId=' + id).then((response) => {
        console.log(response);
        sale = response.data;
      })
      return sale;
    }

    static async createSale(sale) {
      let respSale = {};
      await saleApi.post('/sale/', sale).then((response) => {
        console.log(response);
        respSale = response.data;
      });
      return respSale;
    }

    static async editProductGroup(group) {
      let success = false;
      await productApi.put('/productgroup/' + group.id, group).then((response) => {
        console.log(response);
        success = true;
      });
      return success;
    }

    static async editSale(sale) {
      let success = false;
      await saleApi.put('/sale/' + sale.id, sale).then((response) => {
        console.log(response);
        success = true;
      });
      return success;
    }

    static async deleteSale(saleID) {
      let success = false;
      await saleApi.delete('/sale/' + saleID).then((response) => {
        console.log(response);
        success = true;
      });
      return success;
    }

    static async sellProductFromGroup(productGroup, newOwnerId, quantity) {
      productGroup.sampleProduct.ownerID = newOwnerId;
      console.log(productGroup);
      let sellResponse;
      await productApi.put('/product?groupId=' + productGroup.id + '&quantity=' + quantity.toString(), productGroup.sampleProduct).then((response) => {
        console.log(response);
        sellResponse = response;
      });
      return sellResponse.data;
    }

    static async uploadProduct(product) {
      let uploadResponse;
      await productApi.post('/product/', product).then((response) => {
        console.log(response);
        uploadResponse = response;
      });
      return uploadResponse;
    }

    static async deleteProduct(productID) {
      let success = false;
      await productApi.delete('/product/' + productID).then((response) => {
        console.log(response);
        success = true;
      });
      return success;
    }

    static async getProduct(productID) {
      let product;
      await productApi.get('/product/' + productID).then((response) => {
        console.log(response);
        product = response.data;
      });
      return product;
    }

    static async editProduct(product) {
      let editResponse;
      await productApi.put('/product/' + product.id + '?action=update', product).then((response) => {
        console.log(response);
        editResponse = response;
      });
      return editResponse;
    }

    static async createNewProductGroupWithProduct(sampleRealProduct) {
      let group = {
        id: "",
        ownerID: sampleRealProduct.ownerID,
        sampleProduct: sampleRealProduct,
        quantity: 0
      }
      await productApi.post('/productgroup', group).then(async (response) => {
        console.log(response);
        if (response.status === 201) {
          let product = sampleRealProduct;
          product.groupID = response.data.id;
          await productApi.put('/product/' + product.id + '?action=update', product).then((response) => {
            console.log(response);
            if (response.status === 200) {
              return response;
            }
            else {
              //error handling
            }
          });
        }
        else {
          //error handling
        }
      });
      
    }

    static async deleteProductGroup(id) {
      let success = false;
      await productApi.delete('/productgroup/' + id).then((response) => {
        console.log(response);
        if (response.status === 204)
          success = true;
        else {
          //error
        }
      });
      return success;
    }
}
export default ApiManager;