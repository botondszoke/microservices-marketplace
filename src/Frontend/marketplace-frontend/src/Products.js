import React from "react";
import {Route, Routes, useParams} from "react-router-dom";
import Details from "./Details.js";
import MyProducts from "./MyProducts.js";

function Products() {
    
    const Wrapper = (props) => {
        const params = useParams();
        return <Details id={params.id} mode={params.id.startsWith("g_") ? "editGroup" : "edit"} />;
    }
        
    return(
        <Routes>
            <Route exact path="/" element={<MyProducts />} />
            <Route path="/:id" element={<Wrapper />} />
        </Routes>
    );
    
}
export default Products;