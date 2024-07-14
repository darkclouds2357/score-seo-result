
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

const fetchSearchResults = async (searchText, compareText) => {
  const response = await fetch(`${API_BASE_URL}/api/v1/seo/search`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ searchValue: searchText, compareUrl: compareText })
  });

  if (!response.ok) {
    throw new Error('Network response was not ok');
  }

  const data = await response.json();
  return data;
};

const fetchSearchHistories = async () => {
  const response = await fetch(`${API_BASE_URL}/api/v1/seo/history`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    throw new Error('Network response was not ok');
  }

  const data = await response.json();
  return data;
};

export {
  fetchSearchResults,
  fetchSearchHistories
};
