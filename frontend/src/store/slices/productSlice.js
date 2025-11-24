import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { productsApi } from '../services/api';

export const fetchProducts = createAsyncThunk(
  'products/fetchAll',
  async () => {
    const response = await productsApi.getAll();
    return response.data;
  }
);

export const createProduct = createAsyncThunk(
  'products/create',
  async (productData) => {
    const response = await productsApi.create(productData);
    return response.data;
  }
);

export const updateProduct = createAsyncThunk(
  'products/update',
  async ({ id, data }) => {
    const response = await productsApi.update(id, data);
    return response.data;
  }
);

export const deleteProduct = createAsyncThunk(
  'products/delete',
  async (id) => {
    await productsApi.delete(id);
    return id;
  }
);

const productSlice = createSlice({
  name: 'products',
  initialState: {
    items: [],
    loading: false,
    error: null,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProducts.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchProducts.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchProducts.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      })
      .addCase(createProduct.fulfilled, (state, action) => {
        state.items.push(action.payload);
      })
      .addCase(updateProduct.fulfilled, (state, action) => {
        const index = state.items.findIndex(item => item.itemId === action.payload.itemId);
        if (index !== -1) {
          state.items[index] = action.payload;
        }
      })
      .addCase(deleteProduct.fulfilled, (state, action) => {
        state.items = state.items.filter(item => item.itemId !== action.payload);
      });
  },
});

export default productSlice.reducer;