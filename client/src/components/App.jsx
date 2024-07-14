import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import SeoMatchingPage from '../pages/SeoMatching';
import SearchHistoryPage from '../pages/SearchHistoryPage';
import SearchHistoryDetailPage from '../pages/SearchHistoryDetail';

const App = () => {
    return (
        <Router>
            <div>
                <Routes>
                    <Route exact path="/" element={<SeoMatchingPage />} />
                    <Route exact path="/search-history" element={<SearchHistoryPage />} />
                    <Route path="/search-history/:id" element={<SearchHistoryDetailPage />} />
                </Routes>
            </div>
        </Router>
    );
};

export default App;
