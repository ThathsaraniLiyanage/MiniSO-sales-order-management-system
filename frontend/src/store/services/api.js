import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5041/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor to extract data from wrapped response
api.interceptors.response.use(
  (response) => {
    // If response has data.data, extract it
    if (response.data && response.data.data !== undefined) {
      return { ...response, data: response.data.data };
    }
    return response;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Clients (Customers)
export const customersApi = {
  getAll: () => api.get('/clients'),
  getById: (id) => api.get(`/clients/${id}`),
  create: (data) => api.post('/clients', data),
  update: (id, data) => api.put(`/clients/${id}`, data),
  delete: (id) => api.delete(`/clients/${id}`),
};

// Items (Products)
export const productsApi = {
  getAll: () => api.get('/items'),
  getById: (id) => api.get(`/items/${id}`),
  create: (data) => api.post('/items', data),
  update: (id, data) => api.put(`/items/${id}`, data),
  delete: (id) => api.delete(`/items/${id}`),
};

// Sales Orders
export const ordersApi = {
  getAll: () => api.get('/salesorders'),
  getById: (id) => api.get(`/salesorders/${id}`),
  create: (data) => api.post('/salesorders', data),
  update: (id, data) => api.put(`/salesorders/${id}`, data),
  delete: (id) => api.delete(`/salesorders/${id}`),
};

export default api;