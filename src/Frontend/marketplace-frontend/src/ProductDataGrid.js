import React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import { DataGridPro,
        gridColumnVisibilityModelSelector,
        GridEvents,
        useGridApiRef,
        //useGridApiContext 
      } from '@mui/x-data-grid-pro';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';


const useKeepGroupingColumnsHidden = (apiRef, columns, initialModel,leafField
    ) => {
      const prevModel = React.useRef(initialModel);
    
      React.useEffect(() => {
        apiRef.current.subscribeEvent(GridEvents.rowGroupingModelChange, (newModel) => {
          const columnVisibilityModel = {
            ...gridColumnVisibilityModelSelector(apiRef),
          };
          newModel.forEach((field) => {
            if (!prevModel.current.includes(field)) {
              columnVisibilityModel[field] = false;
            }
          });
          prevModel.current.forEach((field) => {
            if (!newModel.includes(field)) {
              columnVisibilityModel[field] = true;
            }
          });
          apiRef.current.setColumnVisibilityModel(columnVisibilityModel);
          prevModel.current = newModel;
        });
      }, [apiRef]);
    
      return React.useMemo(
        () =>
          columns.map((colDef) =>
            initialModel.includes(colDef.field) ||
            (leafField && colDef.field === leafField)
              ? { ...colDef, hide: true }
              : colDef,
          ),
        [columns, initialModel, leafField],
      );
    };

function ProductDataGrid(props) {
    const apiRef = useGridApiRef();
    const columns = useKeepGroupingColumnsHidden(apiRef, props.dataColumns.concat(props.groupColumns), props.groupColumns.map(c => c.field));

    const getDetailPanelContent = React.useCallback(
        ({ row }) => <DetailPanelContent row={row} 
                                        products={props.products}
                                        delete={props.deleteProduct} 
                                        edit={props.editProduct} 
                                        newGroup={props.newGroup} 
                                        deleteGroup={props.deleteGroup}
                                        removeProductFromGroup={props.removeProductFromGroup}
                                        addProductToGroup={props.addProductToGroup}
                                        manageGroup={props.manageGroup}/>,
        [],
    );
    
    const getDetailPanelHeight = React.useCallback(({row}) => row["id"].startsWith("g_") ? 250 : 60, []);

    //A bonyolult hierarchikus selector a licensz hiányát jelző vízjel eltávolításához szükséges.
    return(
        <Container sx={{height: "100%", textAlign: "center"}}>
            <Typography variant="h3" sx={{margin: "36px auto 0 auto", width: "fit-content"}}><b>My products</b></Typography>
            
            <Divider sx={{margin: "18px 0"}}/>
            <Box sx={{display: "flex", height: "500px"}}>
                <Box sx={{flexGrow: 1, '& > div > div[class] > div[style*="position: absolute"]': {visibility: "hidden"}, '& .group': {bgcolor: "#EEEEEE", fontWeight: "bold"}}}>
                    <DataGridPro
                        rows={props.rows}
                        columns={columns}
                        apiRef = {apiRef}
                        pageSize={5}
                        rowsPerPageOptions={[5]}
                        rowGroupingColumnMode="multiple"
                        defaultGroupingExpansionDepth={1}
                        initialState={{
                            rowGrouping: {
                                model: props.groupColumns.map(c => c.field),
                            },
                        }}
                        groupingColDef={{
                            width: 300
                        }}
                        experimentalFeatures={{
                            rowGrouping: true,
                        }}
                        getRowClassName={(params) => {
                            return params.row["id"].startsWith("g_") ? "group" : "product";
                        }}
                        getDetailPanelContent={getDetailPanelContent}
                        getDetailPanelHeight={getDetailPanelHeight}
                    />
                </Box>
            </Box>
        </Container>
    ); 
}

function DetailPanelContent(props) {
    console.log(props);
    const [showProductsForGroup, setShowProductsForGroup] = React.useState(false);
    const [selectedProduct, setSelectedProduct] = React.useState("");
    const productSelector = [];
    productSelector.push(
        <TextField
            select
            id="selProduct"
            label="Add product"
            value={selectedProduct}
            onChange={(event) => {setSelectedProduct(event.target.value); props.addProductToGroup(event.target.value, props.row["id"]);}}
            sx={{margin: "14px 0 6px 18px", width: "20ch"}}
            color="basic"
            size="small"
        >
            {props.products.map(p => {
                if (p.groupID === null && p.isAvailable === "Available" && p.name === props.row["name"] && p.description === props.row["description"]
                && p.condition === props.row["condition"]) 
                    return <MenuItem value={p.id}>{p.name} - {p.id}</MenuItem>
                return null;
            })}
        </TextField>
    );
    //const apiRef = useGridApiContext();
    if (props.row["id"].startsWith("g_")) {
        return(
            <Paper elevation={1} sx={{margin: "18px 32px", overflow: "auto", maxHeight: "calc(250px - 36px)"}}>
                <Box sx={{textAlign: "left"}}>
                    <Button size="small" variant="outlined" color="basic" sx={{margin: "18px 0 12px 18px"}} onClick={() => props.manageGroup(props.row["id"])}>Manage group</Button>
                    <Button size="small" variant="outlined" color="basic" sx={{margin: "18px 0 12px 18px"}} onClick={() => props.deleteGroup(props.row["id"])} disabled={props.row["isAvailable"] !== "Available"}>Delete group</Button>
                    <Button size="small" variant={showProductsForGroup ? "contained" : "outlined"} color="basic" sx={{margin: "18px 0 12px 18px"}} onClick={() => setShowProductsForGroup(!showProductsForGroup)}>Add product</Button>
                    {showProductsForGroup ? productSelector : null}
                </Box>
                <Divider />
                <Box>
                    {props.products.filter(p => p.groupID === props.row["id"].substring(2)).map((p, idx) => {return(
                        <div key={idx}>
                        <Box sx={{display: "flex"}}>
                            <Typography sx={{margin: "13px 0 13px 20px", display: "inline", fontWeight: "400", fontSize: "0.875rem"}}>{"Item No. " + (idx+1)}</Typography>
                            <Box sx={{flexGrow: 1, display: "inline"}}></Box>
                            <Button size="small" variant="outlined" color="basic" sx={{margin: "9px 18px 9px 18px"}} onClick={() => props.removeProductFromGroup(props.row["id"], idx)}>
                                Remove
                            </Button>
                        </Box>
                        <Divider />
                        </div>
                    );})}
                </Box>
            </Paper>
        );
    }
    return(
        <Paper elevation={1} sx={{margin: "7px 18px", textAlign: "left"}}>
            <Button size="small" variant="outlined" color="basic" sx={{margin: "8px 9px 8px 32px"}} onClick={() => props.edit(props.row["id"])}>Edit</Button>
            <Button size="small" variant="outlined" color="basic" sx={{margin: "8px 9px 8px 9px"}} onClick={() => props.delete(props.row["id"])}>Delete</Button>
            <Button size="small" variant="outlined" color="basic" sx={{margin: "8px 9px 8px 9px"}} onClick={() => props.newGroup(props.products.filter(p=> p.id === props.row["id"])[0])}>Add to new group</Button>
        </Paper>
    );
}

export default ProductDataGrid;