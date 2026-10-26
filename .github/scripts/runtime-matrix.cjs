// Discover every stable release line from the existing CI minimum.
const metadataUrl = 'https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json';
const compare = (a, b) => a.localeCompare(b, 'en', {numeric: true});

module.exports = async function discover({core, fetch = globalThis.fetch}) {
  const response = await fetch(metadataUrl, {
    headers: {'User-Agent': 'sailthru-ci-runtime-matrix'},
    signal: AbortSignal.timeout(30000)
  });
  if (!response.ok) throw new Error(`Runtime metadata request failed: ${response.status}`);
  const releases = await response.json();
  const versions = releases['releases-index']
    .filter(release => release.product === '.NET'
      && /^\d+\.\d+\.\d+$/.test(release['latest-sdk'])
      && /^\d+\.\d+$/.test(release['channel-version']))
    .map(release => release['channel-version'])
    .filter(version => compare(version, '8.0') >= 0)
    .map(version => `${version}.x`);

  const matrix = [...new Set(versions)].sort(compare);
  if (!matrix.includes('8.0.x') || matrix.length > 256) {
    throw new Error('Runtime matrix is incomplete or exceeds the GitHub Actions limit');
  }
  core.info(`Testing dotnet release lines: ${matrix.join(', ')}`);
  core.setOutput('versions', JSON.stringify(matrix));
};
