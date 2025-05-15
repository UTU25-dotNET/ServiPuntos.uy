import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import authService from "../services/authService";

const PrivateRoute = () => {
  const isAuthenticated = authService.isAuthenticated();

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

export default PrivateRoute;
