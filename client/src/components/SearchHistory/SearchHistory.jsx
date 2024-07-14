// components/SearchHistoryPage/SearchHistoryPage.jsx

import React from 'react';
import useFetch from '../../hooks/useFetch';
import { fetchSearchHistories } from '../../api';

const SearchHistory = () => {
  const { data: histories, loading, error } = useFetch(fetchSearchHistories);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      <h2>Search History</h2>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Searched Value</th>
            <th>Compare URL</th>
            <th>Search Engine</th>
            <th>Searched At</th>
            <th>Rank</th>
          </tr>
        </thead>
        <tbody>
          {histories.map((history) => (
            <tr key={history.id}>
              <td>{history.id}</td>
              <td>{history.searchedValue}</td>
              <td>{history.compareUrl}</td>
              <td>{history.searchEngine}</td>
              <td>{new Date(history.searchedAt).toLocaleString()}</td>
              <td>{history.seoRanks.join(', ')}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default SearchHistory;
