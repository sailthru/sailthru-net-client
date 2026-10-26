const {test} = require('node:test');
const assert = require('node:assert/strict');
const discover = require('./runtime-matrix.cjs');

async function resolve(releases) {
  let output;
  await discover({
    core: {info() {}, setOutput(name, value) {
      assert.equal(name, 'versions');
      output = JSON.parse(value);
    }},
    fetch: async () => ({ok: true, json: async () => releases})
  });
  return output;
}

test('includes intermediate and future stable releases, excludes prereleases and duplicates', async () => {
  const releases = {'releases-index': [
    ...[12, 8, 7, 11, 9, 10].map(major => ({product: '.NET',
      'channel-version': `${major}.0`, 'latest-sdk': `${major}.0.100`,
      'support-phase': major < 10 ? 'eol' : 'active'})),
    {product: '.NET', 'channel-version': '13.0', 'latest-sdk': '13.0.100-rc.1'},
    {product: '.NET Framework', 'channel-version': '99.0', 'latest-sdk': '99.0.100'}
  ]};
  assert.deepEqual(await resolve(releases), ['8.0.x', '9.0.x', '10.0.x', '11.0.x', '12.0.x']);
});

test('fails rather than quietly running an empty or incomplete matrix', async () => {
  await assert.rejects(resolve({'releases-index': []}));
  await assert.rejects(resolve({'releases-index': [{product: '.NET', 'channel-version': '12.0', 'latest-sdk': '12.0.100'}]}));
  await assert.rejects(resolve(null));
});

test('metadata HTTP, JSON and network failures fail discovery without emitting a matrix', async () => {
  const core = {info() {}, setOutput() {assert.fail('Must not emit a fallback matrix');}};
  await assert.rejects(discover({core, fetch: async () => ({ok: false, status: 503})}), /503/);
  await assert.rejects(discover({core, fetch: async () => ({
    ok: true, json: async () => {throw new SyntaxError('Invalid JSON');}
  })}), /Invalid JSON/);
  await assert.rejects(discover({core, fetch: async () => {throw new Error('Network failure');}}), /Network failure/);
});
