import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { customersApi } from '../services/api';

export const fetchCustomers = createAsyncThunk(
    'customers/fetchAll',
    async () => {
        const response = await customersApi.getAll();
        return response.data;
    }
);

export const createCustomer = createAsyncThunk(
    'customers/create',
    async (customerData) => {
        const response = await customersApi.create(customerData);
        return response.data;
    }
);

export const updateCustomer = createAsyncThunk(
    'customers/update',
    async ({ id, data }) => {
        const response = await customersApi.update(id, data);
        return response.data;
    }
);

export const deleteCustomer = createAsyncThunk(
    'customers/delete',
    async (id) => {
        await customersApi.delete(id);
        return id;
    }
);

const customerSlice = createSlice({
    name: 'customers',
    initialState: {
        items: [],
        loading: false,
        error: null,
    },
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchCustomers.pending, (state) => {
                state.loading = true;
            })
            .addCase(fetchCustomers.fulfilled, (state, action) => {
                state.loading = false;
                state.items = action.payload;
            })
            .addCase(fetchCustomers.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message;
            })
            .addCase(createCustomer.fulfilled, (state, action) => {
                state.items.push(action.payload);
            })
            .addCase(updateCustomer.fulfilled, (state, action) => {
                const index = state.items.findIndex(item => item.clientId === action.payload.clientId);
                if (index !== -1) {
                    state.items[index] = action.payload;
                }
            })
            .addCase(deleteCustomer.fulfilled, (state, action) => {
                state.items = state.items.filter(item => item.clientId !== action.payload);
            });
    },
});

export default customerSlice.reducer;