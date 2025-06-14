import React from 'react';
import { Link } from 'react-router-dom';

const Breadcrumb = ({ current }) => (
  <div
    style={{
      fontSize: '0.9rem',
      color: '#6c757d',
      marginBottom: '0.5rem',
      display: 'flex',
      alignItems: 'center',
      gap: '0.5rem'
    }}
  >
    <Link to="/" style={{ color: '#007bff', textDecoration: 'none' }}>🏠 Inicio</Link>
    <span>›</span>
    <span>{current}</span>
  </div>
);

export default Breadcrumb;