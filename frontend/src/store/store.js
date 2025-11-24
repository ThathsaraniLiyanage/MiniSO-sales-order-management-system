import { configureStore } from '@reduxjs/toolkit';
import customerReducer from './slices/customerSlice';
import productReducer from './slices/productSlice';
import orderReducer from './slices/orderSlice';

export const store = configureStore({
  reducer: {
    customers: customerReducer,
    products: productReducer,
    orders: orderReducer,
  },
});

export default store;