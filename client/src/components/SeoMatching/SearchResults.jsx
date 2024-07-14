import React from 'react';

const SearchResults = ({ results }) => (
  <table>
    <thead>
      <tr>
        <th>Engine</th>
        <th>Rank</th>
      </tr>
    </thead>
    <tbody>
      {Object.keys(results).map((engine) =>
        results[engine].map((rank, index) => (
          <tr key={`${engine}-${index}`}>
            <td>{engine}</td>
            <td>{rank}</td>
          </tr>
        ))
      )}
    </tbody>
  </table>
);

export default SearchResults;
