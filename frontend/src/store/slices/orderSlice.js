import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { ordersApi } from '../services/api';

export const fetchOrders = createAsyncThunk(
  'orders/fetchAll',
  async () => {
    const response = await ordersApi.getAll();
    return response.data;
  }
);

export const createOrder = createAsyncThunk(
  'orders/create',
  async (orderData) => {
    const response = await ordersApi.create(orderData);
    return response.data;
  }
);

export const updateOrder = createAsyncThunk(
  'orders/update',
  async ({ id, data }) => {
    const response = await ordersApi.update(id, data);
    return response.data;
  }
);

export const deleteOrder = createAsyncThunk(
  'orders/delete',
  async (id) => {
    await ordersApi.delete(id);
    return id;
  }
);

const orderSlice = createSlice({
  name: 'orders',
  initialState: {
    items: [],
    loading: false,
    error: null,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchOrders.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      })
      .addCase(createOrder.fulfilled, (state, action) => {
        state.items.push(action.payload);
      })
      .addCase(updateOrder.fulfilled, (state, action) => {
        const index = state.items.findIndex(item => item.id === action.payload.id);
        if (index !== -1) {
          state.items[index] = action.payload;
        }
      })
      .addCase(deleteOrder.fulfilled, (state, action) => {
        state.items = state.items.filter(item => item.id !== action.payload);
      });
  },
});

export default orderSlice.reducer;