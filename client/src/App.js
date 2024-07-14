import './App.css';
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import SeoMatchingPage from './pages/SeoMatching';
import SearchHistoryPage from './pages/SearchHistoryPage';

function App() {
  return (
    <div className="App">
      <Router>
        <nav className="Breadcrumb">
          <Link to="/">Search</Link> / <Link to="/search-history">Search History</Link>
        </nav>
        <div className="container">
          <Routes>
            <Route exact path="/" element={<SeoMatchingPage />} />
            <Route exact path="/search-history" element={<SearchHistoryPage />} />
          </Routes>
        </div>
      </Router>
    </div>
  );
}

export default App;
