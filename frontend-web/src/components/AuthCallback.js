import React, { useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

const AuthCallback = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    useEffect(() => {
        const token = searchParams.get('token');

        if (token) {
            // Guardar el token JWT recibido del backend
            localStorage.setItem('token', token);
            navigate('/dashboard');
        } else {
            navigate('/login');
        }
    }, [searchParams, navigate]);

    return <div>Procesando autenticación...</div>;
};

export default AuthCallback;