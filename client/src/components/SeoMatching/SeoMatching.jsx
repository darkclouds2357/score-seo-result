import React, { useState } from 'react';
import { fetchSearchResults } from '../../api';
import SearchResults from './SearchResults';
const SeoMatching = () => {
  const [searchText, setSearchText] = useState('');
  const [compareText, setCompareText] = useState('');
  const [results, setResults] = useState([]);

  const handleSearch = async () => {
    try {
      const data = await fetchSearchResults(searchText, compareText);
      setResults(data);
    } catch (error) {
      console.error('Error fetching search results:', error);
      // Handle error state or display error message
    }
  };

  return (
    <div>
      <input
        type="text"
        value={searchText}
        onChange={(e) => setSearchText(e.target.value ?? "land registry search")}
        placeholder="land registry search"
      />
      <input
        type="text"
        value={compareText}
        onChange={(e) => setCompareText(e.target.value ?? "https://www.infotrack.co.uk/")}
        placeholder="https://www.infotrack.co.uk/"
      />
      <button onClick={handleSearch}>Search</button>

      <SearchResults results={results} />
    </div>
  );
};

export default SeoMatching;
