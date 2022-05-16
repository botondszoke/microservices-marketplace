import React from "react";
import {Route, Routes, useParams} from "react-router-dom";
import SaleList from "../pages/SaleList.js"
import SaleDetails from "../pages/SaleDetails.js";

function Sales() {
    
    const Wrapper = (props) => {
        const params = useParams();
        return <SaleDetails id ={params.id} />;
    }
        
    return(
        <Routes>
            <Route exact path="/" element={<SaleList />} />
            <Route path="/:id" element={<Wrapper />} />
        </Routes>
    );
    
}
export default Sales;